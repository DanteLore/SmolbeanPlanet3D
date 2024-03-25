using System;

public class Farmer : SmolbeanColonist
{
    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);

        var giveUpJob = new SwitchColonistToFreeState(this);

        AT(giveUpJob, JobTerminated());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job.IsTerminated;
    }
}
