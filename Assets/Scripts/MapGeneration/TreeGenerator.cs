using System;
using UnityEngine;

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

    private int[] gameMap;
    private int mapWidth;
    private int mapHeight;

    private GridManager gridManager;
    private GameMapGenerator gameMapGenerator;

    public void GenerateTrees()
    {
        Clear();

        gameMapGenerator = GameObject.FindAnyObjectByType<GameMapGenerator>();
        gameMap = gameMapGenerator.GameMap;
        mapWidth = gameMapGenerator.mapWidth;
        mapHeight = gameMapGenerator.mapHeight;

        gridManager = GameObject.FindAnyObjectByType<GridManager>();

        for(int z = 0; z < mapHeight; z++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                if(gameMap[z * mapWidth + x] > 0)
                {
                    CreateTreeAt(z, x);
                }
            }
        }
    }

    private void CreateTreeAt(int z, int x)
    {
        Rect squareBounds = gridManager.GetSquareBounds(x, z);

        int treeIndex = UnityEngine.Random.Range(0, treeData.Length);

        GameObject tree = Instantiate(treeData[treeIndex].prefab);
        tree.transform.parent = treeParent.transform;
        tree.layer = LayerMask.NameToLayer(treeLayer);

        float worldX = UnityEngine.Random.Range(squareBounds.xMin, squareBounds.xMax);
        float worldZ = UnityEngine.Random.Range(squareBounds.yMin, squareBounds.yMax);
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);
        tree.transform.position = new Vector3(worldX, worldY, worldZ);

        float rotationY = UnityEngine.Random.Range(0f, 360f);
        float rotationX = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);
        float rotationZ = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);
        tree.transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        tree.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void Clear()
    {
        while (treeParent.transform.childCount > 0)
            DestroyImmediate(treeParent.transform.GetChild(0).gameObject);
    }
}
