using System.Text;

public class PorterClaimDeliveryRequest : IState
{
    private readonly Porter porter;
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
            ThinkAboutJob();
    }

    private void ThinkAboutJob()
    {
        StringBuilder sb = new();
        sb.Append("Claimed a delivery job: ");
        int min = porter.DeliveryRequest.Minimum;
        int max = porter.DeliveryRequest.Quantity;
        if (min != max)
            sb.Append($"{min}-{max} ");
        else
            sb.Append($"{max} ");
        sb.Append(porter.DeliveryRequest.Item.dropName);
        sb.Append(" to ");
        sb.Append(porter.DeliveryRequest.Building.name);

        porter.Think(sb.ToString());
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
