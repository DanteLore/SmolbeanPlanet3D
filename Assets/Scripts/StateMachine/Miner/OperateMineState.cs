using UnityEngine;
using System;

public class OperateMineState : IState
{
    private Animator animator;
    private Miner miner;
    private Mine mine;
    private SoundPlayer soundPlayer;
    private StateMachine stateMachine;
    public bool Finished { get; private set; }
    private IState startState;

    public void SetFinished(bool val)
    {
        Finished = val;
    }

    public OperateMineState(Miner miner, Animator animator, SoundPlayer soundPlayer)
    {
        this.miner = miner;
        this.animator = animator;
        this.soundPlayer = soundPlayer;

        mine = (Mine)miner.Home;
        
        stateMachine = new StateMachine(shouldLog:true);

        var walkToJob = new WalkDownTunnelState(miner, soundPlayer);
        var doJob = new HarvestResourcesInMineState(miner, soundPlayer);
        var walkBack = new WalkBackUpTunnelState(miner, soundPlayer);
        var finished = new MiningCompleteState(this);

        AT(walkToJob, doJob, WalkedToJob());
        AT(doJob, walkBack, InventoryIsNotEmpty());
        AT(walkBack, finished, WalkedBack());

        startState = walkToJob;

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        Func<bool> WalkedToJob() => () => walkToJob.WalkingTime >= mine.TunnelTime;
        Func<bool> WalkedBack() => () => walkBack.WalkingTime >= mine.TunnelTime;
        Func<bool> InventoryIsNotEmpty() => () => !miner.Inventory.IsEmpty();
    }

    public void OnEnter()
    {
        Finished = false;
        miner.Hide();

        stateMachine.SetState(startState);
    }

    public void OnExit()
    {
        miner.Show();
    }

    public void Tick()
    {
        stateMachine?.Tick();
    }
}
