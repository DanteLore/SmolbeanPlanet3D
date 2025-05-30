using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class ShipwreckManager : MonoBehaviour, IObjectGenerator
{
    public static ShipwreckManager Instance { get; private set; }

    [SerializeField] private BuildingSpec shipwreckSpec;
    [SerializeField] private Ingredient[] startingInventory;
    [SerializeField] private float shipwreckClearingRadius = 6f;
    [SerializeField] private string natureLayer = "Nature";

    public SmolbeanBuilding Shipwreck { get; private set; }
    public int Priority { get { return 200; } }
    public bool RunModeOnly { get { return true; } }


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Clear()
    {
        // Nothing to do here, as the shipwreck will be controlled by the building manager once it's created
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        // Nothing to do here.  Shipwreck is saved as a building once the game has started
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        // Shipwreck will have been loaded as a building, just grab a reference here...
        Shipwreck = BuildingController.Instance.Buildings.FirstOrDefault(b => b is Shipwreck);
        return null;
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        var pos = PlaceShipwreck(gameMap, gameMapWidth, gameMapHeight);
        ClearNatureObjectsAround(pos);
        yield return null;

        FindFirstObjectByType<CameraController>().SetTarget(pos);
        yield return null;
    }

    private Vector3 PlaceShipwreck(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        // Does a shipwreck already exist?
        Shipwreck = BuildingController.Instance.Buildings.FirstOrDefault(b => b is Shipwreck);
        if (Shipwreck != null)
            return Shipwreck.transform.position;

        Vector2Int mapPos = FindShipwreckLocation(gameMap, gameMapWidth, gameMapHeight);

        // Generate the inventory
        var inventory = startingInventory.Select(i => new InventoryItemSaveData { dropSpecName = i.item.dropName, quantity = i.quantity });

        // Place the shipwreck
        Shipwreck = BuildingController.Instance.PlaceBuildingOnSquare(shipwreckSpec, mapPos.x, mapPos.y, inventory);

        return Shipwreck.transform.position;
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
