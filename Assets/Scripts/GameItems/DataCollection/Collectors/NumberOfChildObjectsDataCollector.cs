public class NumberOfChildObjectsDataCollector : DataCollectionSeries
{
    protected override float GetDataValue()
    {
        return transform.childCount;
    }
}