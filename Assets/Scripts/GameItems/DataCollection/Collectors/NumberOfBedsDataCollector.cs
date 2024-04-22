using System.Linq;

public class NumberOfBedsDataCollector : DataCollectionSeries
{
    private BuildingController buildingController;

    private void Start()
    {
        buildingController = GetComponent<BuildingController>();
    }

    protected override float GetDataValue()
    {
        return buildingController.GetAllHomes().Sum(h => h.maxCapacity);
    }
}
