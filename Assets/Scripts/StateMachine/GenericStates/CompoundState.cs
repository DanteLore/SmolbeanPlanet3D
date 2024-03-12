using System;

public abstract class CompoundState : IState
{
    protected readonly StateMachine stateMachine;
    protected IState startState;
    public bool Finished { get; private set; }

    public CompoundState()
    {
        stateMachine = new StateMachine(shouldLog: false);
    }

    public virtual void OnEnter()
    {
        Finished = false;
    }

    public virtual void OnExit()
    {
        Finished = true;
    }

    public virtual void Tick()
    {
        stateMachine?.Tick();
    }

    public void SetFinished(bool val)
    {
        Finished = val;
    }

    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);
}
