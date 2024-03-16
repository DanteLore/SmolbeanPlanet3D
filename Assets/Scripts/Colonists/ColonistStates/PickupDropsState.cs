public class PickupDropsState : IState
{
    private IGatherDrops gatherer;
    private DropController dropController;
    private SmolbeanDrop stack;

    public PickupDropsState(IGatherDrops gatherer, DropController dropController)
    {
        this.gatherer = gatherer;
        this.dropController = dropController;
    }

    public void OnEnter()
    {       
        stack = gatherer.TargetDrop.GetComponent<SmolbeanDrop>();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        ((SmolbeanColonist)gatherer).Think($"Picking up stack of {stack.quantity} {stack.dropSpec.dropName}");
        gatherer.Inventory.PickUp(dropController.Pickup(stack));
        gatherer.TargetDrop = null;
    }
}
