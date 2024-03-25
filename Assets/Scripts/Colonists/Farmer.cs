using System;

public class Farmer : SmolbeanColonist
{
    public float fieldRadius = 4f;

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        // Set target to a point near the barn that has lots of grass
        // While inventory not full
        //      Go to target
        //      Harvest - add wheat to inventory
        //      set target to a point close to current location - in a circle pattern maybe
        // Return to barn, drop inventory

        var idle = new IdleState(animator);
        var giveUpJob = new SwitchColonistToFreeState(this);
        var findFieldCenter = new FarmerSelectFieldCenterState(this, fieldRadius);
        var wander = new WalkToTargetState(this, navAgent, animator, soundPlayer);

        AT(giveUpJob, JobTerminated());

        AT(idle, findFieldCenter, BeenIdleFor(5));
        AT(findFieldCenter, wander, HasSomewhereToGo());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job.IsTerminated;
        Func<bool> BeenIdleFor(float t) => () => idle.TimeIdle > t;
        Func<bool> HasSomewhereToGo() => () => !CloseEnoughTo(target);
    }
}
