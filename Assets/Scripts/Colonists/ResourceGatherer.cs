
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public abstract class ResourceGatherer : MonoBehaviour
{
    public string natureLayer = "Nature";
    public string dropLayer = "Drops";
    public float destinationThreshold = 1.0f;
    public float damage = 20f;
    public float idleTime = 1f;
    public float sleepTime = 2f;
    public DropSpec dropSpec;
    private NavMeshAgent navAgent;
    private GameObject body;
    private Vector3 lastReportedPosition;
    private Vector3 lastPosition;
    private Animator animator;
    private Stack<InventoryItem> inventory;
    private SoundPlayer soundPlayer;
    private StateMachine stateMachine;

    public GameObject Target { get; set; }
    public GameObject DropTarget { get; set; }
    public Vector3 SpawnPoint { get; set; }
    public Vector3 DropPoint { get; set; }

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

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        var hut = GetComponentInParent<SmolbeanBuilding>();
        SpawnPoint = hut.GetSpawnPoint();
        DropPoint = hut.GetDropPoint();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;
        inventory = new Stack<InventoryItem>();
        soundPlayer = GetComponent<SoundPlayer>();

        stateMachine = new StateMachine();
        
        var searchForResources = new SearchForResourceState(this, natureLayer);
        var searchForDrops = new SearchForDropsState(this, dropLayer);

        var walkToResource = new WalkToResourceState(this, navAgent, animator);
        var walkToDrop = new WalkToDropState(this, navAgent, animator);
        var walkHome = new WalkHomeState(this, navAgent, animator);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator);

        var harvestResource = new HarvestResource(this, navAgent, animator, soundPlayer);
        var pickupDrop = new PickupDropsState(this, DropController.Instance);
        var dropInventory = new DropInventoryState(this, DropController.Instance);

        var idle = new IdleState(animator);
        var sleeping = new SleepState(this);
        var waitForTargetToDie = new WaitForTargetToDieState(animator);

        AT(walkToResource,  walkHome, () => walkToResource.StuckTime > 2f);
        AT(walkToDrop,      walkHome, () => walkToDrop.StuckTime > 2f);
        AT(walkToDropPoint, walkHome, () => walkToDropPoint.StuckTime > 2f);

        AT(searchForResources, walkToResource, HasTarget());
        AT(walkToResource, harvestResource, IsCloseEnoughToTarget());
        AT(harvestResource, waitForTargetToDie, TargetIsDying());
        AT(waitForTargetToDie, searchForDrops, TargetIsDead());

        AT(searchForDrops, walkToDrop, DropFound());
        AT(walkToDrop, pickupDrop, IsCloseEnoughToDrop());
        AT(walkToDrop, walkHome, NoDropsFound());
        AT(pickupDrop, walkHome, InventoryEmpty());
        AT(pickupDrop, walkToDropPoint, InventoryNotEmpty());

        AT(walkToDropPoint, dropInventory, IsAtDropPoint());
        AT(dropInventory, walkHome, InventoryEmpty());

        AT(searchForDrops, walkHome, NoDropsFound());
        AT(walkHome, sleeping, IsAtSpawnPoint());
        AT(sleeping, idle, HasBeenSleepingForAWhile());
        AT(idle, searchForResources, HasBeenIdleForAWhile());

        stateMachine.SetState(searchForResources);

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
        Func<bool> HasTarget() => () => Target != null;
        Func<bool> IsCloseEnoughToTarget() => () => CloseEnoughTo(Target);
        Func<bool> IsCloseEnoughToDrop() => () => CloseEnoughTo(DropTarget);
        Func<bool> TargetIsDying() => () => Target != null && Target.GetComponent<IDamagable>().IsDead;
        Func<bool> TargetIsDead() => () => Target == null;
        Func<bool> DropFound() => () => DropTarget != null;
        Func<bool> NoDropsFound() => () => DropTarget == null;
        Func<bool> InventoryEmpty() => () => inventory.Count == 0;
        Func<bool> InventoryNotEmpty() => () => inventory.Any();
        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> IsAtDropPoint() => () => CloseEnoughTo(DropPoint);
        Func<bool> HasBeenIdleForAWhile() => () => idle.TimeIdle >= idleTime;
        Func<bool> HasBeenSleepingForAWhile() => () => sleeping.TimeAsleep >= sleepTime;
    }

    void Update()
    {
        stateMachine.Tick();
        UpdateGroundWear();
    }

    private void UpdateGroundWear()
    {
        if (!body.activeInHierarchy)
            return;

        float threshold = GroundWearManager.Instance.updateThreshold;
        if (Vector3.SqrMagnitude(transform.position - lastReportedPosition) > threshold * threshold)
        {
            lastReportedPosition = transform.position;
            GroundWearManager.Instance.WalkedOn(transform.position);
        }
    }

    public void Hide()
    {
        body.SetActive(false);
    }

    public void Show()
    {
        body.SetActive(true);
    }

    public void PickUp(InventoryItem item)
    {
        inventory.Push(item);
    }

    public InventoryItem DropFirstInInventory()
    {
        return (inventory.Count > 0) ? inventory.Pop() : null;
    }

    private bool DropPointFull()
    {
        int count = Physics.OverlapSphere(DropPoint, 1f, LayerMask.GetMask(dropLayer))
                    .Select(c => c.gameObject.GetComponent<ItemStack>())
                    .Where(i => i != null && i.dropSpec == dropSpec)
                    .Sum(i => i.quantity);

        return count >= dropSpec.stackSize;
    }

    protected bool CloseEnoughTo(Vector3 dest)
    {
        Vector3 v1 = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 v2 = new Vector3(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    protected bool CloseEnoughTo(GameObject target)
    {
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(natureLayer, dropLayer));
        return found.Any(c => c.gameObject == target);
    }
}
