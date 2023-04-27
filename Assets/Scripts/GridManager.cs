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

    private System.Random rand = new System.Random();

    public bool addMeshDebugGizmos = false;

    private MeshData[] map;

    public void Recreate()
    {
        ClearMap();

        var meshData = new MeshLoader(fuzzyEdgeFactor).LoadMeshes();
        print($"Loaded {meshData.Count()} meshes");

        var neighbourData = new NeighbourSelector(fuzzyEdgeFactor, meshData).SelectNeighbours();

        var nd = neighbourData["BasicFloor"];
        //print("Left: " + String.Join(",", nd.leftMatches));
        //print("Right: " + String.Join(",", nd.rightMatches));
        //print("Front: " + String.Join(",", nd.frontMatches));
        //print("Back: " + String.Join(",", nd.backMatches));

        map = new MapGenerator(mapWidth, mapHeight, meshData, neighbourData).GenerateMap();
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

                var tileObj = new GameObject();
                tileObj.transform.position = pos - offset;
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.mesh = map[z * mapWidth + x].mesh;
                var collider = tileObj.AddComponent<MeshCollider>();
                tileObj.GetComponent<Renderer>().material = meshMaterial;

                if(addMeshDebugGizmos)
                    tileObj.AddComponent<DebugMesh>();

                tileObj.transform.parent = transform;
                tileObj.name = $"Terrain cube {x}, 0, {z}";
            }
        }
    }

    private void ClearMap()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
