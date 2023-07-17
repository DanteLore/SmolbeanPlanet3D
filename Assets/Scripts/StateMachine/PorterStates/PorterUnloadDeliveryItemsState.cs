public class PorterUnloadDeliveryItemsState : IState
{
    private Porter porter;
    private readonly DeliveryManager deliveryManager;

    public PorterUnloadDeliveryItemsState(Porter porter, DeliveryManager deliveryManager)
    {
        this.porter = porter;
        this.deliveryManager = deliveryManager;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        var delivery = porter.DeliveryRequest;

        if(porter.Inventory.Contains(delivery.Item, delivery.Quantity))
        {
            var storehouse = (Storehouse)porter.Home;
            var item = porter.Inventory.Take(delivery.Item, delivery.Quantity);

            if(item != null)
            {
                storehouse.Inventory.PickUp(item);
                deliveryManager.LogFinished(porter, porter.DeliveryRequest);
            }
        }
    }
}
