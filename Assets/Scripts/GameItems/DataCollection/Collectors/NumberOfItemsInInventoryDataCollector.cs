using System.Linq;

public class NumberOfItemsInInventoryDataCollector : DataCollectionSeries
{
    public DropSpec dropSpec;

    private BuildingController buildingController;

    private void Start()
    {
        buildingController = GetComponent<BuildingController>();
    }

    protected override float GetDataValue()
    {
        int count = 0;
        foreach(var building in buildingController.Buildings)
            foreach(var item in building.Inventory.Totals)
                if(item.dropSpec == dropSpec)
                    count += item.quantity;
                    
        return count;
    }
}
