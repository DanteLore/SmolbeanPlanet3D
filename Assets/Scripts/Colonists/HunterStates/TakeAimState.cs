public class TakeAimState : IState
{
    readonly Hunter hunter;
    private SmolbeanAnimal target;

    public TakeAimState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public void OnEnter()
    {
        target = hunter.Prey;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        hunter.TakeAim();
    }
}
