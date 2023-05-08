using System;
using UnityEngine;
using UnityEngine.AI;

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

        float xOffset = UnityEngine.Random.Range(0f, 1000f);
        float yOffset = UnityEngine.Random.Range(0f, 1000f);

        gridManager = GameObject.FindAnyObjectByType<GridManager>();

        for(int z = 0; z < mapHeight; z++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                if(gameMap[z * mapWidth + x] > 0)
                {
                    float sample = Mathf.PerlinNoise((x + xOffset) / (mapWidth * noiseScale), (z + yOffset) / (mapHeight * noiseScale));

                    if(sample > noiseThreshold)
                    {
                        CreateTreeAt(z, x);
                    }
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

        float buffer = gridManager.tileSize * edgeBuffer;
        float worldX = UnityEngine.Random.Range(squareBounds.xMin + buffer, squareBounds.xMax - buffer);
        float worldZ = UnityEngine.Random.Range(squareBounds.yMin + buffer, squareBounds.yMax - buffer);
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);
        tree.transform.position = new Vector3(worldX, worldY, worldZ);

        float rotationY = UnityEngine.Random.Range(0f, 360f);
        float rotationX = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);
        float rotationZ = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);
        tree.transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        tree.transform.localScale = new Vector3(scale, scale, scale);

//        var collider = tree.GetComponentInChildren<CapsuleCollider>();
//        var nmo = tree.AddComponent<NavMeshObstacle>();
//        nmo.shape = NavMeshObstacleShape.Capsule;
//        nmo.radius = collider.radius;
//        nmo.height = collider.height;
    }

    public void Clear()
    {
        while (treeParent.transform.childCount > 0)
            DestroyImmediate(treeParent.transform.GetChild(0).gameObject);
    }
}
