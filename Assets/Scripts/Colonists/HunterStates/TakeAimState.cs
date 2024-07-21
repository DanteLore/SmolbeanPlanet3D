using UnityEngine;

public class TakeAimState : IState
{
    private readonly Hunter hunter;
    private float aimStartTime;

    public bool IsReady { get => Time.time - aimStartTime > 2f; }

    public TakeAimState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public void OnEnter()
    {
        aimStartTime = Time.time;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        hunter.TakeAim();
        hunter.Think("Taking aim...");
    }
}
