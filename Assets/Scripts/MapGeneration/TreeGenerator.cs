using System;
using UnityEngine;

public class TreeGenerator : MonoBehaviour, IObjectGenerator
{
    public GameObject treeParent;
    private int[] gameMap;
    private int mapWidth;
    private int mapHeight;

    public void GenerateTrees()
    {
        var gameMapGenerator = GameObject.FindAnyObjectByType<GameMapGenerator>();
        gameMap = gameMapGenerator.GameMap;
        mapWidth = gameMapGenerator.mapWidth;
        mapHeight = gameMapGenerator.mapHeight;

        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                if(gameMap[y * mapWidth + x] > 0)
                {
                    Debug.Log($"Tree on {x} {y}");
                }
            }
        }
    }

    public void Clear()
    {
        while (treeParent.transform.childCount > 0)
            DestroyImmediate(treeParent.transform.GetChild(0).gameObject);
    }
}
