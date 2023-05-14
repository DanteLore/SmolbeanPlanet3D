using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public struct TreeData
{
    public GameObject prefab;
    public float probability;
}

public class TreeGenerator : MonoBehaviour, IObjectGenerator
{
    public GameObject treeParent;
    public TreeData[] treeData;
    public string treeLayer = "Nature";

    public float scaleMax = 1.2f;
    public float scaleMin = 0.8f;
    public float tiltMaxDegrees = 6.0f;
    public float edgeBuffer = 0.1f;

    public float noiseScale = 0.1f;
    public float noiseThreshold = 0.5f;

    private List<int> gameMap;
    private int mapWidth;
    private int mapHeight;

    private GridManager gridManager;
    private GameMapGenerator gameMapGenerator;

    private List<NatureObjectSaveData> saveData;

    void Start()
    {
        gridManager = GameObject.FindAnyObjectByType<GridManager>();
        gameMapGenerator = GameObject.FindAnyObjectByType<GameMapGenerator>();
    }

    public List<NatureObjectSaveData> GetSaveData()
    {
        return saveData;
    }

    public void LoadTrees(List<NatureObjectSaveData> loadedData)
    {
        saveData = loadedData;

        Clear();

        foreach(var treeData in saveData)
            InstantiateTree(treeData);
    }

    public void GenerateTrees()
    {
        saveData = new List<NatureObjectSaveData>();

        Clear();
        
        gameMap = gridManager.GameMap;
        mapWidth = gameMapGenerator.mapWidth;
        mapHeight = gameMapGenerator.mapHeight;

        float xOffset = UnityEngine.Random.Range(0f, 1000f);
        float yOffset = UnityEngine.Random.Range(0f, 1000f);

        for(int z = 0; z < mapHeight; z++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                if(gameMap[z * mapWidth + x] > 0)
                {
                    float sample = Mathf.PerlinNoise((x + xOffset) / (mapWidth * noiseScale), (z + yOffset) / (mapHeight * noiseScale));

                    if(sample > noiseThreshold)
                        saveData.Add(GenerateTreeData(z, x));
                }
            }
        }

        foreach(var treeData in saveData)
            InstantiateTree(treeData);
    }

    private NatureObjectSaveData GenerateTreeData(int z, int x)
    {
        Rect squareBounds = gridManager.GetSquareBounds(x, z);

        int treeIndex = UnityEngine.Random.Range(0, treeData.Length);

        float buffer = gridManager.tileSize * edgeBuffer;
        float worldX = UnityEngine.Random.Range(squareBounds.xMin + buffer, squareBounds.xMax - buffer);
        float worldZ = UnityEngine.Random.Range(squareBounds.yMin + buffer, squareBounds.yMax - buffer);
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

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
        while (treeParent.transform.childCount > 0)
            DestroyImmediate(treeParent.transform.GetChild(0).gameObject);
    }

    private void InstantiateTree(NatureObjectSaveData data)
    {
        var position = new Vector3(data.positionX, data.positionY, data.positionZ);
        var rotation = Quaternion.Euler(data.rotationX, data.rotationY, data.rotationZ);
        var scale = new Vector3(data.scaleX, data.scaleY, data.scaleZ);

        GameObject tree = Instantiate(treeData[data.prefabIndex].prefab);
        tree.transform.parent = treeParent.transform;
        tree.layer = LayerMask.NameToLayer(treeLayer);
        tree.transform.position = position;
        tree.transform.rotation = rotation;
        tree.transform.localScale = scale;
    }
}
