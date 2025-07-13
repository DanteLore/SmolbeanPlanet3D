public class PorterFetchDropsFinished : IState
{
    private PorterFetchDropsState parent;
    private IDeliverDrops porter;
    public string Name { get => GetType().Name; }

    public PorterFetchDropsFinished(PorterFetchDropsState parent, IDeliverDrops porter)
    {
        this.parent = parent;
        this.porter = porter;
    }

    public void OnEnter()
    {
        parent.SetFinished(true);
        DeliveryManager.Instance.CompleteCollectionJob(porter);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
