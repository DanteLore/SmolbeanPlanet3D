using System;
using UnityEngine;
using UnityEngine.AI;

public class PorterFetchDropsState : CompoundState
{
    public PorterFetchDropsState(Porter porter, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer, string dropLayer)
    { 
        var walkToJobStart = new WalkToDropState(porter, navAgent, animator, soundPlayer);
        var pickupDrops = new PickupDropsState(porter, DropController.Instance);
        var walkHome = new WalkHomeState(porter, navAgent, animator, soundPlayer);
        var storeDrops = new PorterStoreDropsState(porter, DropController.Instance);
        var finished = new PorterFetchDropsFinished(this, porter);

        AT(walkToJobStart, pickupDrops, IsCloseEnoughToDrop());
        AT(pickupDrops, walkHome, NoTargetDropAssigned());
        AT(walkHome, storeDrops, IsAtSpawnPoint());
        AT(storeDrops, finished, InventoryIsEmpty());

        AT(walkToJobStart, finished, NoTargetDropAssigned()); // Drop picked up or deleted while I was en route, go home
        AT(walkToJobStart, walkHome, () => walkToJobStart.StuckTime > 2f); // Stuck

        stateMachine.SetStartState(walkToJobStart);

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        Func<bool> NoTargetDropAssigned() => () => porter.TargetDrop == null;
        Func<bool> IsCloseEnoughToDrop() => () => porter.CloseEnoughTo(porter.TargetDrop);
        Func<bool> IsAtSpawnPoint() => () => porter.CloseEnoughTo(porter.Job.Building.spawnPoint);
        Func<bool> InventoryIsEmpty() => porter.Inventory.IsEmpty;
    }
}
