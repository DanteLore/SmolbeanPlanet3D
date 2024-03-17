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
        porter.DeliveryRequest = deliveryManager.ClaimNextDeliveryRequest(porter, ((Storehouse)porter.Job.Building).Inventory);
        if (porter.DeliveryRequest != null)
            porter.Think($"Claimed a delivery job: {porter.DeliveryRequest.Quantity} {porter.DeliveryRequest.Item.dropName} to {porter.DeliveryRequest.Building.name}");
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
