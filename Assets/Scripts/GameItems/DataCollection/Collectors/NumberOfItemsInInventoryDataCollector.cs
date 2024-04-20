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
        return buildingController.Buildings
            .SelectMany(b => b.Inventory.Totals)
            .Where(i => i.dropSpec == dropSpec)
            .Sum(i => i.quantity);
    }
}
