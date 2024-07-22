using System;

public class GenericState : IState
{
    private readonly string displayName;
    private readonly Action onEnter;
    private readonly Action onExit;
    private readonly Action tick;

    public GenericState(string displayName = "GenericState", Action onEnter = null, Action onExit = null, Action tick = null)
    {
        this.displayName = displayName;
        this.onEnter = onEnter;
        this.onExit = onExit;
        this.tick = tick;
    }

    public void OnEnter()
    {
        onEnter?.Invoke();
    }

    public void OnExit()
    {
        onExit?.Invoke();
    }

    public void Tick()
    {
        tick?.Invoke();
    }

    public override string ToString()
    {
        return displayName;
    }
}