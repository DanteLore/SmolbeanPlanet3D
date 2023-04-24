using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public Material meshMaterial;

    public int mapWidth = 10;
    public int mapHeight = 10;
    public float tileSize = 4.0f;
    public float fuzzyEdgeFactor = 0.01f;

    private Dictionary<String, NeighbourData> neighbourData;
    private System.Random rand = new System.Random();

    private MeshData[] map;

    public void Recreate()
    {
        ClearMap();

        var meshData = new MeshLoader(fuzzyEdgeFactor).LoadMeshes();
        print($"Loaded {meshData.Count()} meshes");

        neighbourData = new NeighbourSelector(fuzzyEdgeFactor, meshData).SelectNeighbours();

        map = new MapGenerator(mapWidth, mapHeight, meshData).GenerateMap();

        DrawMap();
    }

    private void DrawMap()
    {
        Vector3 offset = new Vector3((mapWidth * tileSize) / 2, 0.0f, (mapHeight * tileSize) / 2);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                // In future it might make sense to look at creating one big mesh here, rather than separate game objects... maybe.
                var pos = new Vector3(x * tileSize, 0, z * tileSize);
                Mesh mesh = GetMeshFor(x, z);

                var tileObj = new GameObject();
                tileObj.transform.position = pos - offset;
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var collider = tileObj.AddComponent<MeshCollider>();
                //collider.sharedMesh = floorMesh;
                tileObj.GetComponent<Renderer>().material = meshMaterial;

                tileObj.transform.parent = transform;
                tileObj.name = $"Terrain cube {x}, 0, {z}";
            }
        }
    }

    private Mesh GetMeshFor(int x, int z)
    {
        return map[z * mapWidth + x].mesh;
    }

    private void ClearMap()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
