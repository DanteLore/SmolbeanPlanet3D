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
    [SerializeField] private Arrow arrowPrefab;
    [SerializeField] private Vector3 targetPointOffset;
    [SerializeField] private float nockDuration = 0.1f;
    [SerializeField] private float aimDuration = 10f;
    [SerializeField] private float armLength = 1.5f;
    [SerializeField] private float arrowSpawnOffset = 0.8f;
    [SerializeField] private float targetRotationOffset = 90f;

    public Vector3 BowPosition { get; private set; }
    public Quaternion BowRotation { get; private set; }
    public bool BowActive { get; private set; }

    public SmolbeanAnimal Prey { get; set; }
    public bool IsAimReady => BowActive && Time.time - aimStartTime > aimDuration;
    public bool IsRecoverDone => !BowActive && Time.time - recoverStartTime > aimDuration;
    public float IKWeight
    {
        get
        {
            if (BowActive)
                return Mathf.Clamp01((Time.time - aimStartTime) / aimDuration);
            return Mathf.Clamp01(1f - (Time.time - recoverStartTime) / aimDuration);
        }
    }

    private float aimStartTime;
    private float recoverStartTime;
    private float armHeight;
    private float chosenShotHeight;
    private Vector3 preyTargetPoint;
    private Arrow arrow;

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        armHeight = CalculateArmHeight();

        var gridManager = FindAnyObjectByType<GridManager>();
        Bounds bounds = gameObject.GetRendererBounds();
        float halfMyHeight = bounds.max.y - bounds.min.y;

        var idle = new IdleState(animator);
        var waitForTargetToDie = new IdleState(animator);
        var searchForPrey = new SearchForPreyState(this, targetSpecies, creatureLayer);
        var searchForShootingSpot = new SearchForShootingSpotState(this, gridManager, halfMyHeight, targetPointOffset, natureLayer, groundLayer, shotDistance);
        var nockArrow = new NockArrowState(this, nockDuration);
        var takeAim = new TakeAimState(this, soundPlayer);
        var shoot = new GenericState("Shoot", onEnter: Shoot);
        var recover = new RecoverState(this);
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
        AT(walkToTarget, nockArrow, InPosition());
        AT(walkToTarget, searchForPrey, StuckGettingToShootingPosition());
        AT(nockArrow, takeAim, NockReady());
        AT(takeAim, shoot, Ready());
        AT(shoot, recover, ShotDone());
        AT(shoot, recover, ArrowLost());
        AT(recover, waitForTargetToDie, RecoverDone());
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
        Func<bool> NockReady() => () => nockArrow.IsReady && Prey != null;
        Func<bool> Ready() => () => takeAim.IsReady && Prey != null;
        Func<bool> ShotDone() => () => arrow != null && !arrow.Flying;
        Func<bool> ArrowLost() => () => arrow == null;
        Func<bool> RecoverDone() => () => recover.IsReady;
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

    protected override void Update()
    {
        base.Update();

        if (IKWeight <= 0f || Prey == null)
            return;

        BowPosition = transform.position + Vector3.up * armHeight + LaunchDirection() * armLength;
        BowRotation = Quaternion.LookRotation(LaunchDirection(), Vector3.up) * Quaternion.Euler(0f, 0f, 90f);
    }

    public void UpdateAiming()
    {
        if (Prey == null)
            return;

        transform.LookAt(Prey.transform.position);
        transform.Rotate(0f, targetRotationOffset, 0f);
    }

    public void ChooseShotHeight()
    {
        chosenShotHeight = Random.Range(minShotHeight, maxShotHeight);
    }

    public void StartAiming()
    {
        BowActive = true;
        aimStartTime = Time.time;
    }

    public void StopAiming()
    {
        BowActive = false;
    }

    public void StartRecovering()
    {
        BowActive = false;
        recoverStartTime = Time.time;
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

    public void Shoot()
    {
        Think("Fire!");
        soundPlayer.Play("LooseArrow");

        Vector3 origin = transform.position + Vector3.up * armHeight + LaunchDirection() * arrowSpawnOffset;
        preyTargetPoint = Prey.transform.GetRendererBounds().center + targetPointOffset;
        float distanceY = preyTargetPoint.y - origin.y;
        float time = CalculateTimeToTarget(chosenShotHeight, Physics.gravity.y, distanceY);
        Vector3 horizontalDirection = preyTargetPoint - origin;
        horizontalDirection.y = 0f;

        arrow = Instantiate(arrowPrefab, origin, Quaternion.identity);
        arrow.GetComponent<Rigidbody>().linearVelocity = CalculateInitialVelocity(chosenShotHeight, horizontalDirection, Physics.gravity.y, time);
    }

    private Vector3 LaunchDirection()
    {
        Vector3 origin = transform.position + Vector3.up * armHeight;
        Vector3 targetPos = Prey.transform.GetRendererBounds().center + targetPointOffset;
        float distanceY = targetPos.y - origin.y;
        float time = CalculateTimeToTarget(chosenShotHeight, Physics.gravity.y, distanceY);
        Vector3 horizontalDirection = targetPos - origin;
        horizontalDirection.y = 0f;
        return CalculateInitialVelocity(chosenShotHeight, horizontalDirection, Physics.gravity.y, time).normalized;
    }

    private float CalculateArmHeight()
    {
        Transform shoulder = body.transform.FindDeepChild("Shoulder.L");
        Transform foot = body.transform.FindDeepChild("Foot.L");

        if (shoulder == null || foot == null)
        {
            Debug.LogWarning("Hunter: could not find Shoulder.L or Foot.L bones, defaulting armHeight to 0.9");
            return 0.9f;
        }

        return shoulder.position.y - foot.position.y;
    }

    private float CalculateTimeToTarget(float height, float gravity, float distanceY)
    {
        return Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (distanceY - height) / gravity);
    }

    private Vector3 CalculateInitialVelocity(float height, Vector3 horizontalDirection, float gravity, float time)
    {
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = horizontalDirection * (1 / time);
        return -Mathf.Sign(gravity) * (velocityY + velocityXZ);
    }
}
