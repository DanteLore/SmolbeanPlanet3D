using System;

public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        target = transform.position; // We start where we want to be

        var idle = new IdleState(animator);
        var graze = new GrazeState(this, animator, navAgent, soundPlayer);
        var flock = new FlockState(this, animator, navAgent, soundPlayer);

        AT(idle, Dead()); // If we're dead, do nothing!
        AT(flock, graze, Hungry());
        AT(graze, flock, Full());
        
        stateMachine.SetState(flock);

        Func<bool> Dead() => () => isDead;
        Func<bool> Hungry() => IsHungry;
        Func<bool> Full() => IsFull;
    }

    public override bool IsEnoughFoodHere()
    {
        return GroundWearManager.Instance.GetAvailableGrass(transform.position) >= 0.1f;
    }

    protected override void Update()
    {
        base.Update();
    }
}
