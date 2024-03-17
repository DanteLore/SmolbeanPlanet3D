using System;

public class OperateMineState : CompoundState
{
    private readonly Miner miner;

    protected Mine Mine
    {
        get
        {
            return (Mine)miner.Job.Building;
        }
    }

    public OperateMineState(Miner miner, SoundPlayer soundPlayer): base()
    {
        this.miner = miner;

        var walkToJob = new WalkDownTunnelState(miner, soundPlayer);
        var doJob = new HarvestResourcesInMineState(miner, soundPlayer);
        var walkBack = new WalkBackUpTunnelState(miner, soundPlayer);
        var finished = new MiningCompleteState(this);

        AT(walkToJob, doJob, WalkedToJob());
        AT(doJob, walkBack, InventoryIsNotEmpty());
        AT(walkBack, finished, WalkedBack());

        stateMachine.SetStartState(walkToJob);

        Func<bool> WalkedToJob() => () => walkToJob.WalkingTime >= Mine.TunnelTime;
        Func<bool> WalkedBack() => () => walkBack.WalkingTime >= Mine.TunnelTime;
        Func<bool> InventoryIsNotEmpty() => () => !miner.Inventory.IsEmpty();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        miner.Hide();
        Mine.StartMining();
    }

    public override void OnExit()
    {
        if(Mine != null) // Mine *could* have been destroyed while we were underground!
            Mine.StopMining();
        miner.Show();
        base.OnExit();
    }
}
