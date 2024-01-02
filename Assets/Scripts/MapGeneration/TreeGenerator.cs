using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TreeGenerator : MonoBehaviour, IObjectGenerator
{
    [Serializable]
    public struct TreeData
    {
        public GameObject prefab;
        public float probability;
    }
    public int Priority { get { return 10; } }

    public TreeData[] treeData;
    public int maxTreesPerSquare = 4;
    public float minimumTreeAltitude = 1.0f;
    public float scaleMax = 1.2f;
    public float scaleMin = 0.8f;
    public float tiltMaxDegrees = 6.0f;
    public float edgeBuffer = 0.1f;
    public float noiseScale = 0.1f;
    public float noiseThreshold = 0.5f;
    public MapData mapData;
    public string groundLayer = "Ground";

    private GridManager gridManager;

    public List<NatureObjectSaveData> GetSaveData()
    {
        return GetComponentsInChildren<SmolbeanTree>().Select(t => t.saveData).ToList();
    }

    public void LoadTrees(List<NatureObjectSaveData> loadedData)
    {
        Clear();

        foreach(var treeData in loadedData)
            InstantiateTree(treeData);
    }

    public void Generate(List<int> gameMap, int mapWidth, int mapHeight)
    {
        gridManager = GameObject.FindAnyObjectByType<GridManager>();

        var treeData = new List<NatureObjectSaveData>();

        Clear();

        float xOffset = UnityEngine.Random.Range(0f, 1000f);
        float yOffset = UnityEngine.Random.Range(0f, 1000f);

        for(int z = 0; z < mapHeight; z++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                if(gameMap[z * mapWidth + x] > 0)
                {
                    for(int t = 0; t < maxTreesPerSquare; t++)
                    {
                        float sample = Mathf.PerlinNoise((x + xOffset) / (mapWidth * noiseScale), (z + yOffset) / (mapHeight * noiseScale));

                        if(sample > noiseThreshold)
                        {
                            var data = GenerateTreeData(z, x);
                            if(TreeNotInTheSea(data))
                                treeData.Add(data);
                        }
                    }
                }
            }
        }

        treeData.ForEach(InstantiateTree);
    }

    private bool TreeNotInTheSea(NatureObjectSaveData data)
    {
        Ray ray = new Ray(new Vector3(data.positionX, 100f, data.positionZ), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, LayerMask.GetMask(groundLayer)))
        {
            return hit.point.y > minimumTreeAltitude;
        }
        else
        {
            return false;
        }
    }

    private NatureObjectSaveData GenerateTreeData(int z, int x)
    {
        Rect squareBounds = gridManager.GetSquareBounds(x, z);

        int treeIndex = UnityEngine.Random.Range(0, treeData.Length);

        float buffer = gridManager.tileSize * edgeBuffer;
        float worldX = UnityEngine.Random.Range(squareBounds.xMin + buffer, squareBounds.xMax - buffer);
        float worldZ = UnityEngine.Random.Range(squareBounds.yMin + buffer, squareBounds.yMax - buffer);
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

        if(float.IsNaN(worldY))
            throw new Exception("No grid");

        float rotationY = UnityEngine.Random.Range(0f, 360f);
        float rotationX = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);
        float rotationZ = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);

        return new NatureObjectSaveData
        {
            positionX = worldX,
            positionY = worldY,
            positionZ = worldZ,
            rotationX = rotationX,
            rotationY = rotationY,
            rotationZ = rotationZ,
            scaleX = scale,
            scaleY = scale,
            scaleZ = scale,
            prefabIndex = treeIndex
        };
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    private void InstantiateTree(NatureObjectSaveData data)
    {
        var position = new Vector3(data.positionX, data.positionY, data.positionZ);
        var rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);
        var scale = new Vector3(data.scaleX, data.scaleY, data.scaleZ);

        GameObject tree = Instantiate(treeData[data.prefabIndex].prefab, position, rotation, transform);
        tree.transform.localScale = scale;
        tree.GetComponent<SmolbeanTree>().saveData = data;
    }
}
