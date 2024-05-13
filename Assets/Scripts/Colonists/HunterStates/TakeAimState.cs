using UnityEngine;

public class TakeAimState : IState
{
    private readonly Hunter hunter;
    private SmolbeanAnimal target;
    private float aimStartTime;

    public bool IsReady { get => Time.time - aimStartTime > 2f; }

    public TakeAimState(Hunter hunter)
    {
        this.hunter = hunter;
    }

    public void OnEnter()
    {
        target = hunter.Prey;
        aimStartTime = Time.time;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        hunter.TakeAim();
    }
}
