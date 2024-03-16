public class PorterClaimDeliveryRequest : IState
{
    private Porter porter;
    private readonly DeliveryManager deliveryManager;

    public PorterClaimDeliveryRequest(Porter porter, DeliveryManager deliveryManager)
    {
        this.porter = porter;
        this.deliveryManager = deliveryManager;
    }

    public void OnEnter()
    {
        porter.DeliveryRequest = deliveryManager.ClaimNextDeliveryRequest(porter, ((Storehouse)porter.Home).Inventory);
        porter.Think("Claimed a delivery job");
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
