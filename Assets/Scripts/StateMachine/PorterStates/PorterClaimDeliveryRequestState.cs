public class PorterClaimDeliveryRequest : IState
{
    private Porter porter;
    private readonly DeliveryManager deliveryManager;

    public PorterClaimDeliveryRequest(Porter parent, DeliveryManager deliveryManager)
    {
        this.porter = parent;
        this.deliveryManager = deliveryManager;
    }

    public void OnEnter()
    {
        porter.DeliveryRequest = deliveryManager.ClaimNextRequest(porter, ((Storehouse)porter.Home).Inventory);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
