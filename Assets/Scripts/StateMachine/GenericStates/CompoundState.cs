using System;

public abstract class CompoundState : IState
{
    protected readonly StateMachine stateMachine;
    public bool Finished { get; private set; }

    public CompoundState()
    {
        stateMachine = new StateMachine(shouldLog: false);
    }

    public virtual void OnEnter()
    {
        Finished = false;
        stateMachine.Restart();
    }

    public virtual void OnExit()
    {
        // As we close, call OnExit on the current child state to close things off nicely
        stateMachine.ForceStop();

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
