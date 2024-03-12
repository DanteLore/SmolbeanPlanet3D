using System;

public class OperateMineState : CompoundState
{
    private readonly Miner miner;
    private readonly Mine mine;

    public OperateMineState(Miner miner, SoundPlayer soundPlayer): base()
    {
        mine = (Mine)miner.Home;

        var walkToJob = new WalkDownTunnelState(miner, soundPlayer);
        var doJob = new HarvestResourcesInMineState(miner, soundPlayer);
        var walkBack = new WalkBackUpTunnelState(miner, soundPlayer);
        var finished = new MiningCompleteState(this);

        AT(walkToJob, doJob, WalkedToJob());
        AT(doJob, walkBack, InventoryIsNotEmpty());
        AT(walkBack, finished, WalkedBack());

        startState = walkToJob;

        Func<bool> WalkedToJob() => () => walkToJob.WalkingTime >= mine.TunnelTime;
        Func<bool> WalkedBack() => () => walkBack.WalkingTime >= mine.TunnelTime;
        Func<bool> InventoryIsNotEmpty() => () => !miner.Inventory.IsEmpty();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        miner.Hide();
        mine.StartMining();

        stateMachine.SetState(startState);
    }

    public override void OnExit()
    {
        mine.StopMining();
        miner.Show();
        base.OnExit();
    }
}
