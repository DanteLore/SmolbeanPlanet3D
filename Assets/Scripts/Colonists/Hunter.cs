using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hunter : ResourceGatherer, IDeliverDrops
{
    [SerializeField] private float shotDistance = 16f;
    [SerializeField] private AnimalSpec targetSpecies;
    [SerializeField] private GameObject bow;
    [SerializeField] private Arrow arrowPrefab;

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
        var searchForPrey = new SearchForPreyState(this, gridManager, targetSpecies, halfMyHeight, creatureLayer, shotDistance);
        var takeAim = new TakeAimState(this);
        var shoot = new ShootState(this);
        var walkToTarget = new WalkToTargetState(this, navAgent, animator, soundPlayer);
        var searchForDrops = new SearchForDropsState(this, dropLayer);
        var giveUpJob = new SwitchColonistToFreeState(this);

        AT(giveUpJob, JobTerminated());

        AT(idle, searchForPrey, IdleFor(5f));

        AT(searchForPrey, walkToTarget, TargetFound());
        AT(searchForPrey, idle, NoTargetFound());

        AT(walkToTarget, takeAim, InPosition());
        AT(takeAim, shoot, Ready());

        AT(shoot, idle, ShotDone());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job == null || Job.IsTerminated;
        Func<bool> IdleFor(float s) => () => idle.TimeIdle > s;
        Func<bool> TargetFound() => () => !searchForPrey.InProgress && Prey != null;
        Func<bool> NoTargetFound() => () => !searchForPrey.InProgress && Prey == null;
        Func<bool> InPosition() => () => Prey != null && CloseEnoughTo(Target, 2f);
        Func<bool> Ready() => () => takeAim.IsReady;
        Func<bool> ShotDone() => () => true;
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
        Vector3 targetPointOffset = Vector3.up * 0.2f;
        var bowPos = transform.position + transform.rotation * new Vector3(0f, 1f, 1f);
        float shotHeight = Random.Range(0.1f, 0.3f);
        Vector3 targetPoint = Prey.transform.GetRendererBounds().center + targetPointOffset;
        float distanceY = targetPoint.y - bowPos.y;
        float time = CalculateTimeToTarget(shotHeight, Physics.gravity.y, distanceY);
        Vector3 horizontalDirection = targetPoint - bowPos;
        horizontalDirection.y = 0f;

        var arrow = Instantiate(arrowPrefab, bowPos, Quaternion.identity);
        Rigidbody arrowBody = arrow.GetComponent<Rigidbody>();
        arrowBody.velocity = CalculateInitialVelocity(shotHeight, horizontalDirection, Physics.gravity.y, time);
    }

    float CalculateTimeToTarget(float height, float gravity, float distanceY)
    {
        return Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (distanceY - height) / gravity);
    }

    Vector3 CalculateInitialVelocity(float height, Vector3 horizontalDirection, float gravity, float time)
    {
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = horizontalDirection * (1 / time);
        Vector3 velocityFinal = velocityY + velocityXZ;
        velocityFinal *= -Mathf.Sign(gravity);
        return velocityFinal;
    }
}
