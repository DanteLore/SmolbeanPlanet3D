using System;

public class Hunter : ResourceGatherer, IDeliverDrops
{
    public SmolbeanAnimal Prey { get; set; }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);
        var searchForPrey = new SearchForPreyState(this);
        var attack = new AttackPreyState(this);
        var searchForDrops = new SearchForDropsState(this, dropLayer);
        var giveUpJob = new SwitchColonistToFreeState(this);

        AT(giveUpJob, JobTerminated());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job == null || Job.IsTerminated;
    }
}
