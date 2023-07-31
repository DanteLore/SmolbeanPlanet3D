using System;

public class Builder : Colonist
{
    private StateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine(shouldLog:false);

        var idle = new IdleState(animator);
        var sleep = new SleepState(this);

        AT(idle, sleep, IdleForAWhile());
        AT(sleep, idle, () => sleep.TimeAsleep > 5f);

        stateMachine.SetState(idle);

        Func<bool> IdleForAWhile() => () => idle.TimeIdle > 8f;

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
