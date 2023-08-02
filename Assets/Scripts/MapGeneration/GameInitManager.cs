using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class StartingShipwreckManager : MonoBehaviour, IObjectGenerator
{
    public BuildingSpec shipwreckSpec;
    public Ingredient[] startingInventory;
    public float shipwreckClearingRadius = 6f;
    public string natureLayer = "Nature";

    public int Priority { get { return 200; } }

    public void Clear()
    {
        // Nothing to do here, as the shipwreck will be controlled by the building manager once it's created
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        // Don't do this at design time
        if (!Application.isPlaying)
            return;
        PlaceShipwreck(gameMap, gameMapWidth, gameMapHeight);

    }

    private void PlaceShipwreck(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        // Does a shipwreck exist?
        if (BuildManager.Instance.Buildings.Any(b => b is Shipwreck))
            return;

        Vector2Int mapPos = FindShipwreckLocation(gameMap, gameMapWidth, gameMapHeight);

        // Generate the inventory
        var inventory = startingInventory.Select(i => new InventoryItemSaveData { dropSpecName = i.item.dropName, quantity = i.quantity });

        // Place the shipwreck
        var building = BuildManager.Instance.PlaceBuildingOnSquare(shipwreckSpec, mapPos.x, mapPos.y, inventory);

        // Clear some space
        ClearNatureObjectsAround(building.transform.position);
    }

    private static Vector2Int FindShipwreckLocation(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        for(int y = 1; y < gameMapHeight - 1; y++)
        {
            for(int x = 1; x < gameMapWidth - 1; x++)
            {
                if(SquareLooksGood(gameMap, x, y, gameMapWidth, gameMapHeight))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogWarning("Could not find a suitable square to place the shipwreck");
        return new Vector2Int(gameMapWidth / 2, gameMapHeight / 2);
    }

    private static bool SquareLooksGood(List<int> gameMap, int centerX, int centerY, int gameMapWidth, int gameMapHeight)
    {
        var levels = new int[3] {0, 0, 0};
        for(int y = centerY - 1; y <= centerY + 1; y++)
        {
            for(int x = centerX - 1; x <= centerX + 1; x++)
            {
                int level = gameMap[y * gameMapWidth + x];
                levels[level]++;
            }
        }

        return levels[0] > 0 && levels[1] >= 6;
    }

    private void ClearNatureObjectsAround(Vector3 pos)
    {
        foreach(var obj in Physics.OverlapSphere(pos, shipwreckClearingRadius, LayerMask.GetMask(natureLayer)))
            Destroy(obj.gameObject);
    }
}
