public class PorterUnloadDeliveryItemsState : IState
{
    private Porter porter;

    public PorterUnloadDeliveryItemsState(Porter porter)
    {
        this.porter = porter;
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

        var item = porter.Inventory.Take(delivery.Item, delivery.Quantity);
        delivery.Building.Inventory.PickUp(item);

        delivery.SetComplete(true);
    }
}
