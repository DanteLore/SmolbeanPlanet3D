public class DeliveryRequest
{
    private SmolbeanBuilding building;
    private DropSpec item;
    private int quantity;
    private int priority;

    public SmolbeanBuilding Building { get { return building; }}
    public DropSpec Item { get { return item; }}
    public int Quantity { get { return quantity; }}
    public int Priority { get { return priority; }}

    public bool IsComplete { get; private set; }

    public DeliveryRequest(SmolbeanBuilding building, DropSpec item, int quantity, int priority)
    {
        this.building = building;
        this.item = item;
        this.quantity = quantity;
        this.priority = priority;
    }

    public void SetComplete(bool val)
    {
        IsComplete = true;
    }
}
