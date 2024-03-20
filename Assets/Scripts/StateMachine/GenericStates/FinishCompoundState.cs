public class FinishCompoundState : IState
{
    protected CompoundState parent;

    public FinishCompoundState(CompoundState parent)
    {
        this.parent = parent;
    }

    public virtual void OnEnter()
    {
        parent.SetFinished(true);
    }

    public virtual void OnExit()
    {
    }

    public virtual void Tick()
    {
    }
}