using System;

public class GenericState : IState
{
    private readonly Action onEnter;
    private readonly Action onExit;
    private readonly Action tick;

    public GenericState(Action onEnter = null, Action onExit = null, Action tick = null)
    {
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
}