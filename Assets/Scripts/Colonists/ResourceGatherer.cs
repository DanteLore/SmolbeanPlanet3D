using UnityEngine;
using System.Linq;
using System;

public abstract class ResourceGatherer : Colonist, IGatherDrops, IReturnDrops
{
    public float damage = 20f;
    public float hitCooldown = 1f;
    public float idleTime = 1f;
    public float sleepTime = 2f;
    public DropSpec dropSpec;
    private StateMachine stateMachine;

    public GameObject Target { get; set; }
    public GameObject TargetDrop { get; set; }
    public Vector3 DropPoint { get; private set; }

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

        DropPoint = Home.GetDropPoint();

        stateMachine = new StateMachine();
        
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
        var sleeping = new SleepState(this);
        var waitForTargetToDie = new WaitForTargetToDieState(animator);

        AT(walkToResource,  walkHome,   () => walkToResource.StuckTime > 5f);
        AT(walkToDrop,      walkHome,   () => walkToDrop.StuckTime > 5f);
        AT(walkToDropPoint, walkHome,   () => walkToDropPoint.StuckTime > 5f);

        AT(searchForResources,  walkToResource,     HasTarget());
        AT(walkToResource,      harvestResource,    IsCloseEnoughToTarget());
        AT(harvestResource,     waitForTargetToDie, TargetIsDying());
        AT(waitForTargetToDie,  searchForDrops,     TargetIsDead());

        AT(searchForDrops,  walkToDrop,         DropFound());
        AT(walkToDrop,      pickupDrop,         IsCloseEnoughToDrop());
        AT(walkToDrop,      walkHome,           NoDropsFound());
        AT(pickupDrop,      walkHome,           InventoryEmpty());
        AT(pickupDrop,      walkToDropPoint,    InventoryNotEmpty());

        AT(walkToDropPoint, dropInventory,  IsAtDropPoint());
        AT(dropInventory,   walkHome,       InventoryEmpty());

        AT(searchForDrops,  walkHome,           NoDropsFound());
        AT(walkHome,        sleeping,           IsAtSpawnPoint());
        AT(sleeping,        idle,               HasBeenSleepingForAWhile());
        AT(idle,            searchForResources, HasBeenIdleForAWhile());

        stateMachine.SetState(searchForResources);

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> IsCloseEnoughToTarget() => () => CloseEnoughTo(Target);
        Func<bool> IsCloseEnoughToDrop() => () => CloseEnoughTo(TargetDrop);
        Func<bool> TargetIsDying() => () => Target != null && Target.GetComponent<IDamagable>().IsDead;
        Func<bool> TargetIsDead() => () => Target == null;
        Func<bool> DropFound() => () => TargetDrop != null;
        Func<bool> NoDropsFound() => () => TargetDrop == null;
        Func<bool> InventoryEmpty() => () => Inventory.IsEmpty();
        Func<bool> InventoryNotEmpty() => () => !Inventory.IsEmpty();
        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> IsAtDropPoint() => () => CloseEnoughTo(DropPoint);
        Func<bool> HasBeenIdleForAWhile() => () => idle.TimeIdle >= idleTime;
        Func<bool> HasBeenSleepingForAWhile() => () => sleeping.TimeAsleep >= sleepTime;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }

    private bool DropPointFull()
    {
        int count = Physics.OverlapSphere(DropPoint, 1f, LayerMask.GetMask(dropLayer))
                    .Select(c => c.gameObject.GetComponent<ItemStack>())
                    .Where(i => i != null && i.dropSpec == dropSpec)
                    .Sum(i => i.quantity);

        return count >= dropSpec.stackSize;
    }
}
