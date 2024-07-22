using UnityEngine;

public class TakeAimState : IState
{
    private readonly Hunter hunter;
    private readonly SoundPlayer soundPlayer;
    private float aimStartTime;

    public bool IsReady { get => Time.time - aimStartTime > 1f; }

    public TakeAimState(Hunter hunter, SoundPlayer soundPlayer)
    {
        this.hunter = hunter;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        aimStartTime = Time.time;
        hunter.Think("Taking aim...");
        soundPlayer.Play("BowStretch");
    }

    public void OnExit()
    {
        soundPlayer.Stop("BowStretch");
    }

    public void Tick()
    {
        hunter.TakeAim();
    }
}
