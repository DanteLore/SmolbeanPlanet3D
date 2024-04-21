using System.Linq;

public class NumberOfUnclaimedDeliveryRequestsDataCollector : DataCollectionSeries
{
    private DeliveryManager deliveryManager;

    private void Start()
    {
        deliveryManager = GetComponent<DeliveryManager>();
    }

    protected override float GetDataValue()
    {
        return deliveryManager.UnclaimedDeliveryRequests.Count();
    }
}
