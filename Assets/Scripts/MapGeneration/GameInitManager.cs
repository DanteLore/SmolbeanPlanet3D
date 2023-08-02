using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        PlaceShipwreck(gameMapWidth, gameMapHeight);

    }

    private void PlaceShipwreck(int gameMapWidth, int gameMapHeight)
    {
        // Does a shipwreck exist?
        if (BuildManager.Instance.Buildings.Any(b => b is Shipwreck))
            return;
        Vector2Int mapPos = FindShipwreckLocation(gameMapWidth, gameMapHeight);

        // Generate the inventory
        var inventory = startingInventory.Select(i => new InventoryItemSaveData { dropSpecName = i.item.dropName, quantity = i.quantity });

        // Place the shipwreck
        var building = BuildManager.Instance.PlaceBuildingOnSquare(shipwreckSpec, mapPos.x, mapPos.y, inventory);

        // Clear some space
        ClearNatureObjectsAround(building.transform.position);
    }

    private static Vector2Int FindShipwreckLocation(int gameMapWidth, int gameMapHeight)
    {
        // Find a square near water with no nature objects on it
        return new Vector2Int(gameMapWidth / 2, gameMapHeight / 2);
    }

    private void ClearNatureObjectsAround(Vector3 pos)
    {
        foreach(var obj in Physics.OverlapSphere(pos, shipwreckClearingRadius, LayerMask.GetMask(natureLayer)))
            Destroy(obj.gameObject);
    }
}
