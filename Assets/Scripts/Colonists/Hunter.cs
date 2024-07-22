using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hunter : ResourceGatherer, IDeliverDrops
{
    [SerializeField] private float shotDistance = 16f;
    [SerializeField] private float minShotHeight = 1f;
    [SerializeField] private float maxShotHeight = 3f;
    [SerializeField] private AnimalSpec targetSpecies;
    [SerializeField] private GameObject bow;
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Vector3 targetPointOffset;
    private Vector3 preyTargetPoint;
    private Arrow arrow;

    public SmolbeanAnimal Prey { get; set; }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        //StateMachine.ShouldLog = true;

        var gridManager = FindFirstObjectByType<GridManager>();
        Bounds bounds = gameObject.GetRendererBounds();
        float halfMyHeight = bounds.max.y - bounds.min.y;

        var idle = new IdleState(animator);
        var waitForTargetToDie = new IdleState(animator);
        var searchForPrey = new SearchForPreyState(this, targetSpecies, creatureLayer);
        var searchForShootingSpot = new SearchForShootingSpotState(this, gridManager, halfMyHeight, targetPointOffset, natureLayer, groundLayer, shotDistance);
        var takeAim = new TakeAimState(this, soundPlayer);
        var shoot = new GenericState("Shoot", onEnter: Shoot);
        var walkToTarget = new WalkToTargetState(this, navAgent, animator, soundPlayer);
        var searchForDrops = new GenericState("SearchForDrops", onEnter: SearchForDropStart, tick: SearchForDropTick);
        var giveUpJob = new SwitchColonistToFreeState(this);
        var walkToDrop = new WalkToDropState(this, navAgent, animator, soundPlayer);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var pickupDrop = new PickupDropsState(this, DropController.Instance);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);

        AT(giveUpJob, JobTerminated());

        AT(idle, searchForPrey, IdleFor(2f));

        AT(searchForPrey, searchForShootingSpot, TargetFound());
        AT(searchForPrey, idle, NoTargetFound());

        AT(searchForShootingSpot, walkToTarget, SpotPicked());
        AT(searchForShootingSpot, idle, NoSpotFound());

        AT(walkToTarget, takeAim, InPosition());
        AT(walkToTarget, searchForPrey, StuckGettingToShootingPosition());

        AT(takeAim, shoot, Ready());

        AT(shoot, waitForTargetToDie, ShotDone());
        AT(shoot, waitForTargetToDie, ArrowLost());

        AT(waitForTargetToDie, searchForDrops, TargetDiedAfter(0.1f));
        AT(waitForTargetToDie, searchForShootingSpot, TargetDidNotDieAfter(0.1f));

        AT(searchForDrops, walkToDrop, DropFound());
        AT(walkToDrop, pickupDrop, IsCloseEnoughToDrop());
        AT(walkToDrop, walkHome, NoDropsFound());
        AT(walkToDrop, walkHome, StuckGettingToDrop());
        AT(pickupDrop, walkHome, InventoryEmpty());
        AT(pickupDrop, walkToDropPoint, InventoryNotEmpty());

        AT(walkToDropPoint, dropInventory, IsAtDropPoint());
        AT(dropInventory, walkHome, InventoryEmpty());

        AT(searchForDrops, walkHome, NoDropsFound());
        AT(walkHome, idle, IsAtSpawnPoint());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job == null || Job.IsTerminated;
        Func<bool> IdleFor(float s) => () => idle.TimeIdle > s;
        Func<bool> TargetFound() => () => !searchForPrey.InProgress && Prey != null;
        Func<bool> SpotPicked() => () => !searchForShootingSpot.InProgress && searchForShootingSpot.Found;
        Func<bool> NoSpotFound() => () => !searchForShootingSpot.InProgress && !searchForShootingSpot.Found;
        Func<bool> NoTargetFound() => () => !searchForPrey.InProgress && Prey == null;
        Func<bool> InPosition() => () => Prey != null && CloseEnoughTo(Target, 2f);
        Func<bool> Ready() => () => takeAim.IsReady && Prey != null;
        Func<bool> ShotDone() => () => arrow != null && !arrow.Flying;
        Func<bool> ArrowLost() => () => arrow == null;
        Func<bool> StuckGettingToShootingPosition() => () => walkToTarget.StuckTime > 10f * Time.timeScale;
        Func<bool> StuckGettingToDrop() => () => walkToDrop.StuckTime > 10f * Time.timeScale;
        Func<bool> TargetDiedAfter(float s) => () => waitForTargetToDie.TimeIdle > s && Prey == null;
        Func<bool> TargetDidNotDieAfter(float s) => () => waitForTargetToDie.TimeIdle > s && Prey != null;
        Func<bool> DropFound() => () => TargetDrop != null;
        Func<bool> NoDropsFound() => () => TargetDrop == null;
        Func<bool> IsCloseEnoughToDrop() => () => CloseEnoughTo(TargetDrop, 1f);
        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(Job.Building.spawnPoint, 1f);
        Func<bool> IsAtDropPoint() => () => CloseEnoughTo(Job.Building.dropPoint, 1f);
        Func<bool> InventoryEmpty() => () => Inventory.IsEmpty();
        Func<bool> InventoryNotEmpty() => () => !Inventory.IsEmpty();
    }

    private void SearchForDropTick()
    {
        if (!TargetDrop)
        {
            var target = GetDropTarget();

            if (target != null)
            {
                TargetDrop = target.gameObject;
                Think($"Picking up {target.quantity} {target.dropSpec.dropName}");
            }
        }
    }

    private SmolbeanDrop GetDropTarget()
    {
        return Physics.OverlapSphere(preyTargetPoint, 10f, LayerMask.GetMask(dropLayer))
            .Select(c => c.gameObject.GetComponent<SmolbeanDrop>())
            .Where(i => i != null && i.dropSpec == DropSpec)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - preyTargetPoint))
            .FirstOrDefault();
    }

    private void SearchForDropStart()
    {
        TargetDrop = null;
    }

    public void TakeAim()
    {
        transform.LookAt(Prey.transform.position);
        // Gonna have to do some funky stuff with animations and IK here one day :S
        // Maybe an animation...
        // Whatever
    }

    public void Shoot()
    {
        Think("Fire!");
        soundPlayer.Play("LooseArrow");

        Vector3 bowPos = transform.position + transform.rotation * new Vector3(0f, 1f, 1f);
        float shotHeight = Random.Range(minShotHeight, maxShotHeight);
        preyTargetPoint = Prey.transform.GetRendererBounds().center + targetPointOffset;
        float distanceY = preyTargetPoint.y - bowPos.y;
        float time = CalculateTimeToTarget(shotHeight, Physics.gravity.y, distanceY);
        Vector3 horizontalDirection = preyTargetPoint - bowPos;
        horizontalDirection.y = 0f;

        arrow = Instantiate(arrowPrefab, bowPos, Quaternion.identity);
        Rigidbody arrowBody = arrow.GetComponent<Rigidbody>();
        arrowBody.linearVelocity = CalculateInitialVelocity(shotHeight, horizontalDirection, Physics.gravity.y, time);
    }

    private float CalculateTimeToTarget(float height, float gravity, float distanceY)
    {
        return Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (distanceY - height) / gravity);
    }

    private Vector3 CalculateInitialVelocity(float height, Vector3 horizontalDirection, float gravity, float time)
    {
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = horizontalDirection * (1 / time);
        Vector3 velocityFinal = velocityY + velocityXZ;
        velocityFinal *= -Mathf.Sign(gravity);
        return velocityFinal;
    }
}
