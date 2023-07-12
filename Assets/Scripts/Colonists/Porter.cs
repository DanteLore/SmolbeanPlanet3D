using System;
using UnityEngine;

public class Porter : Colonist, IGatherDrops, IDeliverDrops
{    
    public float idleTime = 1f;
    public float sleepTime = 2f;

    public GameObject TargetDrop { get; set; }

    private StateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine(shouldLog:false);

        var idle = new IdleState(animator);
        var sleeping = new SleepState(this);

        var searchForJob = new SearchForPorterJobsState(this, dropLayer);
        var walkToJobStart = new WalkToDropState(this, navAgent, animator, soundPlayer);
        var pickupDrops = new PickupDropsState(this, DropController.Instance);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var storeDrops = new StoreDropsState(this, DropController.Instance);

        AT(searchForJob, walkToJobStart, DropFound());
        AT(walkToJobStart, pickupDrops, IsCloseEnoughToDrop());
        AT(pickupDrops, walkHome, NoDropFound());
        AT(walkHome, storeDrops, IsAtSpawnPoint());
        AT(storeDrops, sleeping, InventoryIsEmpty());
        AT(sleeping, idle, HasBeenSleepingForAWhile());
        AT(idle, searchForJob, HasBeenIdleForAWhile());

        AT(walkToJobStart, searchForJob, NoDropFound()); // Drop picked up or deleted while I was en route, find a new one
        AT(walkToJobStart, walkHome, () => walkToJobStart.StuckTime > 2f); // Stuck

        stateMachine.SetState(searchForJob);

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        Func<bool> DropFound() => () => TargetDrop != null;
        Func<bool> NoDropFound() => () => TargetDrop == null;
        Func<bool> IsCloseEnoughToDrop() => () => CloseEnoughTo(TargetDrop);
        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> InventoryIsEmpty() => Inventory.IsEmpty;
        Func<bool> HasBeenIdleForAWhile() => () => idle.TimeIdle >= idleTime;
        Func<bool> HasBeenSleepingForAWhile() => () => sleeping.TimeAsleep >= sleepTime;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
