public class PorterFinishedDeliveryRequestState : IState
{
    private PorterDoDeliveryRequestState parent;
    private Porter porter;
    private readonly DeliveryManager deliveryManager;

    public PorterFinishedDeliveryRequestState(PorterDoDeliveryRequestState parent, Porter porter, DeliveryManager deliveryManager)
    {
        this.parent = parent;
        this.porter = porter;
        this.deliveryManager = deliveryManager;
    }

    public void OnEnter()
    {
        deliveryManager.CompleteDelivery(porter, porter.DeliveryRequest);
        porter.DeliveryRequest = null;
        parent.SetFinished(true);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
