public class DeliveryRequest
{
    private SmolbeanBuilding building;
    private DropSpec item;
    private int quantity;
    private int priority;
    private int minimum;

    public SmolbeanBuilding Building { get { return building; }}
    public DropSpec Item { get { return item; }}
    public int Quantity { get { return quantity; }}
    public int Priority { get { return priority; } set { priority = value; }}
    public int Minimum { get { return minimum; }}
    
    public bool IsComplete { get; private set; }

    public DeliveryRequest(SmolbeanBuilding building, DropSpec item, int quantity, int priority, int minimum = -1)
    {
        this.building = building;
        this.item = item;
        this.quantity = quantity;
        this.priority = priority;
        this.minimum = minimum > 0 ? minimum : quantity;
    }

    public void SetComplete(bool val)
    {
        IsComplete = true;
    }

    public override string ToString()
    {
        return $"DeliveryRequest for {quantity} of {item.dropName} at p={priority}";
    }
}
