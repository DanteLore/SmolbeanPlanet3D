using System;
using UnityEngine;

public class Hunter : ResourceGatherer, IDeliverDrops
{
    [SerializeField] private float shotDistance = 16f;
    [SerializeField] private AnimalSpec targetSpecies;
    [SerializeField] private GameObject bow;

    public SmolbeanAnimal Prey { get; set; }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        StateMachine.ShouldLog = true;

        var gridManager = FindFirstObjectByType<GridManager>();
        Bounds bounds = gameObject.GetRendererBounds();
        float halfMyHeight = bounds.max.y - bounds.min.y;

        var idle = new IdleState(animator);
        var searchForPrey = new SearchForPreyState(this, gridManager, targetSpecies, halfMyHeight, creatureLayer, shotDistance);
        var attack = new TakeAimState(this);
        var walkToTarget = new WalkToTargetState(this, navAgent, animator, soundPlayer);
        var searchForDrops = new SearchForDropsState(this, dropLayer);
        var giveUpJob = new SwitchColonistToFreeState(this);

        AT(giveUpJob, JobTerminated());

        AT(idle, searchForPrey, IdleFor(5f));

        AT(searchForPrey, walkToTarget, TargetFound());
        AT(searchForPrey, idle, NoTargetFound());

        AT(walkToTarget, attack, InPosition());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job == null || Job.IsTerminated;
        Func<bool> IdleFor(float s) => () => idle.TimeIdle > s;
        Func<bool> TargetFound() => () => !searchForPrey.InProgress && Prey != null;
        Func<bool> NoTargetFound() => () => !searchForPrey.InProgress && Prey == null;
        Func<bool> InPosition() => () => Prey != null && CloseEnoughTo(Target, 2f);
    }

    public void TakeAim()
    {
        transform.LookAt(Prey.transform.position);
        // Gonna have to do some funky stuff with animations and IK here one day :S
        // Maybe an animation...
        // Whatever
    }
}
