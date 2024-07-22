using UnityEngine;

public class TakeAimState : IState
{
    private readonly Hunter hunter;
    private float aimStartTime;

    public bool IsReady { get => Time.time - aimStartTime > 1f; }

    public TakeAimState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public void OnEnter()
    {
        aimStartTime = Time.time;
        hunter.Think("Taking aim...");
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        hunter.TakeAim();
    }
}
