using System;
using UnityEngine;

public abstract class StaticResourceGatherer : ResourceGatherer
{
    public Type TargetType
    {
        get
        {
            return GetTargetType();
        }
    }

    public string GatheringTrigger
    {
        get
        {
            return GetGatheringTrigger();
        }
    }

    protected abstract string GetGatheringTrigger();

    protected abstract Type GetTargetType();

    protected override void Start()
    {
        base.Start();

        var giveUpJob = new SwitchColonistToFreeState(this);

        var searchForResources = new SearchForResourceState(this, natureLayer);
        var searchForDrops = new SearchForDropsState(this, dropLayer);

        var walkToResource = new WalkToResourceState(this, navAgent, animator, soundPlayer);
        var walkToDrop = new WalkToDropState(this, navAgent, animator, soundPlayer);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);

        var harvestResource = new HarvestResource(this, navAgent, animator, soundPlayer);
        var pickupDrop = new PickupDropsState(this, DropController.Instance);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);

        var idle = new IdleState(animator);
        var waitForTargetToDie = new WaitForTargetToDieState(animator);

        AT(giveUpJob, JobTerminated());

        AT(walkToResource,  walkHome, () => walkToResource.StuckTime > 10f * Time.timeScale);
        AT(walkToDrop,      walkHome, () => walkToDrop.StuckTime > 10f * Time.timeScale);
        AT(walkToDropPoint, walkHome, () => walkToDropPoint.StuckTime > 10f * Time.timeScale);

        AT(idle, searchForResources, ReadyToGo());
           
        AT(searchForResources,  walkToResource,     HasTarget());
        AT(searchForResources,  idle,               NoTargetFound());
        AT(walkToResource,      harvestResource,    IsCloseEnoughToTarget());
        AT(walkToResource,      searchForResources, CanNotReachTarget());
        AT(harvestResource,     waitForTargetToDie, TargetIsDying());
        AT(harvestResource,     waitForTargetToDie, TargetIsDead());
        AT(waitForTargetToDie,  searchForDrops,     TargetIsDead());

        AT(searchForDrops,  walkToDrop,      DropFound());
        AT(walkToDrop,      pickupDrop,      IsCloseEnoughToDrop());
        AT(walkToDrop,      walkHome,        NoDropsFound());
        AT(pickupDrop,      walkHome,        InventoryEmpty());
        AT(pickupDrop,      walkToDropPoint, InventoryNotEmpty());

        AT(walkToDropPoint, dropInventory, IsAtDropPoint());
        AT(dropInventory,   walkHome,      InventoryEmpty());

        AT(searchForDrops,  walkHome, NoDropsFound());
        AT(walkHome,        idle,     IsAtSpawnPoint());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job.IsTerminated;
        Func<bool> HasTarget() => () => ResourceTarget != null;
        Func<bool> NoTargetFound() => () => ResourceTarget == null && !searchForResources.InProgress;
        Func<bool> IsCloseEnoughToTarget() => () => CloseEnoughTo(ResourceTarget, 2f * ResourceTarget.transform.localScale.x);
        Func<bool> IsCloseEnoughToDrop() => () => CloseEnoughTo(TargetDrop, 1f);
        Func<bool> CanNotReachTarget() => () => walkToResource.IsStuck && !CloseEnoughTo(ResourceTarget, 1f);
        Func<bool> TargetIsDying() => () => ResourceTarget != null && ResourceTarget.GetComponent<IDamagable>().IsDead;
        Func<bool> TargetIsDead() => () => ResourceTarget == null;
        Func<bool> DropFound() => () => TargetDrop != null;
        Func<bool> NoDropsFound() => () => TargetDrop == null;
        Func<bool> InventoryEmpty() => () => Inventory.IsEmpty();
        Func<bool> InventoryNotEmpty() => () => !Inventory.IsEmpty();
        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(Job.Building.spawnPoint, 1f);
        Func<bool> IsAtDropPoint() => () => CloseEnoughTo(Job.Building.dropPoint, 1f);
        Func<bool> ReadyToGo() => () => idle.TimeIdle >= idleTime && !DropPointFull();
    }
}
