using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public Material meshMaterial;
    public float tileSize = 4.0f;
    public float fuzzyEdgeFactor = 0.01f;

    [Range(0.0f, 1.0f)]
    public float coastRadius = 0.8f;

    private System.Random rand = new System.Random();

    public bool addMeshDebugGizmos = false;

    private MeshData[] map;

    private int drawMapWidth;
    private int drawMapHeight;

    public void Recreate()
    {
        GameMapGenerator gameMapGenerator = GetComponent<GameMapGenerator>();
        int gameMapWidth = gameMapGenerator.mapWidth;
        int gameMapHeight = gameMapGenerator.mapHeight;
        drawMapWidth = gameMapWidth + 1;
        drawMapHeight = gameMapHeight + 1;

        ClearMap();

        var meshData = new MeshLoader(fuzzyEdgeFactor).LoadMeshes();
        print($"Loaded {meshData.Count()} meshes");

        var neighbourData = new NeighbourSelector(fuzzyEdgeFactor, meshData).SelectNeighbours();

        var nd = neighbourData["SeaSlopeToCliffTransition"];
        Debug.Log("Left: " + String.Join(", ", nd.leftMatches));
        Debug.Log("Right: " + String.Join(", ", nd.rightMatches));
        Debug.Log("Front: " + String.Join(", ", nd.frontMatches));
        Debug.Log("Back: " + String.Join(", ", nd.backMatches));

        map = new MapGenerator(gameMapWidth, gameMapHeight, coastRadius, meshData, neighbourData).GenerateMap(gameMapGenerator.GameMap);
        DrawMap();
    }

    private void DrawMap()
    {
        Vector3 offset = new Vector3((drawMapWidth * tileSize) / 2, 0.0f, (drawMapHeight * tileSize) / 2);

        for (int x = 0; x < drawMapWidth; x++)
        {
            for (int z = 0; z < drawMapHeight; z++)
            {
                // In future it might make sense to look at creating one big mesh here, rather than separate game objects... maybe.
                var pos = new Vector3(x * tileSize, 0, z * tileSize);
                MeshData meshData = map[z * drawMapWidth + x];

                var tileObj = new GameObject();
                tileObj.transform.position = pos - offset;
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.mesh = meshData.mesh;
                var collider = tileObj.AddComponent<MeshCollider>();
                tileObj.GetComponent<Renderer>().material = meshMaterial;

                if(addMeshDebugGizmos)
                    tileObj.AddComponent<DebugMesh>();

                tileObj.transform.parent = transform;
                tileObj.name = $"({x}, 0, {z}) {meshData.name}";
            }
        }
    }

    private void ClearMap()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}
