using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class GameInitManager : MonoBehaviour, IObjectGenerator
{
    public CameraController cameraController;
    public BuildingSpec shipwreckSpec;
    public Ingredient[] startingInventory;
    public float shipwreckClearingRadius = 6f;
    public string natureLayer = "Nature";

    public int Priority { get { return 200; } }
    public bool RunModeOnly { get { return true; } }

    public void Clear()
    {
        // Nothing to do here, as the shipwreck will be controlled by the building manager once it's created
    }

    public void SaveTo(SaveFileData saveData)
    {
        // Nothing to do here.  Shipwreck is saved as a building once the game has started
    }

    public IEnumerator Load(SaveFileData data)
    {
        // Nothing to do here.  Shipwreck is saved as a building once the game has started
        return null;
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        var pos = PlaceShipwreck(gameMap, gameMapWidth, gameMapHeight);
        ClearNatureObjectsAround(pos);
        yield return null;

        cameraController.gameStartPositon = pos;
        yield return null;
    }

    private Vector3 PlaceShipwreck(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        // Does a shipwreck already exist?
        var shipwreck = BuildingController.Instance.Buildings.FirstOrDefault(b => b is Shipwreck);
        if (shipwreck != null)
            return shipwreck.transform.position;

        Vector2Int mapPos = FindShipwreckLocation(gameMap, gameMapWidth, gameMapHeight);

        // Generate the inventory
        var inventory = startingInventory.Select(i => new InventoryItemSaveData { dropSpecName = i.item.dropName, quantity = i.quantity });

        // Place the shipwreck
        shipwreck = BuildingController.Instance.PlaceBuildingOnSquare(shipwreckSpec, mapPos.x, mapPos.y, inventory);

        return shipwreck.transform.position;
    }

    private static Vector2Int FindShipwreckLocation(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        for(int y = 1; y < gameMapHeight - 1; y++)
        {
            for(int x = 1; x < gameMapWidth - 1; x++)
            {
                if(SquareLooksGood(gameMap, x, y, gameMapWidth))
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogWarning("Could not find a suitable square to place the shipwreck");
        return new Vector2Int(gameMapWidth / 2, gameMapHeight / 2);
    }

    private static bool SquareLooksGood(List<int> gameMap, int centerX, int centerY, int gameMapWidth)
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
