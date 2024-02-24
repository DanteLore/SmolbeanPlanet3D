using System;

public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);
        var eat = new EatGrassState(this);
        var wander = new WanderState(this, navAgent, animator, soundPlayer);

        AT(idle, eat, Hungry());
        AT(eat, wander, Full());
        AT(wander, idle, Arrived());
        AT(idle, wander, IdleFor(5));

        stateMachine.SetState(idle);

        Func<bool> IdleFor(float seconds) => () => idle.TimeIdle > seconds;
        Func<bool> Hungry() => () => foodLevel < 70f;
        Func<bool> Full() => () => foodLevel > 80f;
        Func<bool> Arrived() => () => CloseEnoughTo(target);
    }

    protected override void Update()
    {
        base.Update();
    }
}
