public class TakeAimState : IState
{
    private readonly Hunter hunter;
    private readonly SoundPlayer soundPlayer;

    public string Name { get => GetType().Name; }
    public bool IsReady { get => hunter.IsAimReady; }

    public TakeAimState(Hunter hunter, SoundPlayer soundPlayer)
    {
        this.hunter = hunter;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        hunter.Think("Taking aim...");
        soundPlayer.Play("BowStretch");
        hunter.ChooseShotHeight();
        hunter.StartAiming();
    }

    public void OnExit()
    {
        soundPlayer.Stop("BowStretch");
    }

    public void Tick()
    {
        hunter.UpdateAiming();
    }
}
