public class MiningCompleteState : IState
{
    private OperateMineState parent;

    public MiningCompleteState(OperateMineState parent)
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
