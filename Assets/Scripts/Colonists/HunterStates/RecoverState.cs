public class RecoverState : IState
{
    private readonly Hunter hunter;

    public string Name { get => GetType().Name; }
    public bool IsReady { get => hunter.IsRecoverDone; }

    public RecoverState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public void OnEnter()
    {
        hunter.Think("Recovering...");
        hunter.StartRecovering();
    }

    public void OnExit() { }

    public void Tick() { }
}
