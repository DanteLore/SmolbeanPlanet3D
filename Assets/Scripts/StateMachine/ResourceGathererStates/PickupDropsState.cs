public class PickupDropsState : IState
{
    private ResourceGatherer gatherer;
    private DropController dropController;
    private ItemStack stack;

    public PickupDropsState(ResourceGatherer gatherer, DropController dropController)
    {
        this.gatherer = gatherer;
        this.dropController = dropController;
    }

    public void OnEnter()
    {       
        stack = gatherer.DropTarget.GetComponent<ItemStack>();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        gatherer.Inventory.PickUp(dropController.Pickup(stack));
        gatherer.DropTarget = null;
    }
}