using UnityEngine;

public class NockArrowState : IState
{
    private readonly Hunter hunter;
    private readonly float duration;
    private float enterTime;

    public string Name { get => GetType().Name; }
    public bool IsReady { get => Time.time - enterTime > duration; }

    public NockArrowState(Hunter hunter, float duration)
    {
        this.hunter = hunter;
        this.duration = duration;
    }

    public void OnEnter()
    {
        enterTime = Time.time;
        hunter.Think("Nocking arrow...");
    }

    public void OnExit() { }

    public void Tick()
    {
        hunter.UpdateAiming();
    }
}
