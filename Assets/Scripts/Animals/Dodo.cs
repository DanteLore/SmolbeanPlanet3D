using System;

public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);
        var eat = new EatGrassState(this);

        AT(idle, eat, Hungry());
        AT(eat, idle, Full());

        stateMachine.SetState(idle);

        //Func<bool> IdleFor(float seconds) => () => idle.TimeIdle > seconds;
        Func<bool> Hungry() => () => foodLevel < 70f;
        Func<bool> Full() => () => foodLevel > 80f;
    }

    protected override void Update()
    {
        base.Update();
    }
}
