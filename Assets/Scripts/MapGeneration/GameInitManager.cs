using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StartingShipwreckManager : MonoBehaviour, IObjectGenerator
{
    public BuildingSpec shipwreckSpec;

    public Ingredient[] startingInventory;

    public int Priority { get { return 200; } }

    public void Clear()
    {
        // Nothing to do here, as the shipwreck will be controlled by the building manager once it's created
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        Debug.Log("GAME INIT!");

        // Don't do this at design time
        if(!Application.isPlaying)
            return;

        // Does a shipwreck exist?
        if(BuildManager.Instance.Buildings.Any(b => b is Shipwreck))
            return;

        // Find a square near water with no nature objects on it
        int x = gameMapWidth / 2;
        int y = gameMapHeight / 2;

        // Generate the inventory
        var inventory = startingInventory.Select(i => new InventoryItemSaveData { dropSpecName = i.item.dropName, quantity = i.quantity } );

        // Place the shipwreck
        BuildManager.Instance.PlaceBuildingOnSquare(shipwreckSpec, x, y, inventory);

    }
}
