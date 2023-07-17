public class PorterFetchDropsFinished : IState
{
    private PorterFetchDropsState parent;

    public PorterFetchDropsFinished(PorterFetchDropsState parent)
    {
        this.parent = parent;
    }

    public void OnEnter()
    {
        parent.SetFinished(true);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
