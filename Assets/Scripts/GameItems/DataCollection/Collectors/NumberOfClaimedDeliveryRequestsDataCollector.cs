using System.Linq;

public class NumberOfClaimedDeliveryRequestsDataCollector : DataCollectionSeries
{
    private DeliveryManager deliveryManager;

    private void Start()
    {
        deliveryManager = GetComponent<DeliveryManager>();
    }

    protected override float GetDataValue()
    {
        return deliveryManager.ClaimedDeliveryRequests.Count();
    }
}
