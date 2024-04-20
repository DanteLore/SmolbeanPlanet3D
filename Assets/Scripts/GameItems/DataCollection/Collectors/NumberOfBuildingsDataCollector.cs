using System.Linq;

public class NumberOfBuildingsDataCollector : DataCollectionSeries
{
    private BuildingController buildingController;

    private void Start()
    {
        buildingController = GetComponent<BuildingController>();
    }

    protected override float GetDataValue()
    {
        return buildingController.Buildings.Count();
    }
}
