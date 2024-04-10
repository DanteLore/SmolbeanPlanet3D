using System;
using UnityEngine;

public class Farmer : SmolbeanColonist, IReturnDrops
{
    public DropSpec dropSpec;
    public float fieldRadius = 4f;
    public Vector3 fieldCenter;
    public float grassHarvested;

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        // Set target to a point near the barn that has lots of grass
        // While inventory not full
        //      Go to target
        //      Harvest - add wheat to inventory
        //      set target to a point close to current location - in a circle pattern maybe
        // Return to barn, drop inventory

        var idle = new IdleState(animator);
        var giveUpJob = new SwitchColonistToFreeState(this);
        var findFieldCenter = new FarmerSelectFieldCenterState(this, fieldRadius);
        var harvest = new FarmerHarvestState(this, animator);
        var walkToField = new WalkToTargetState(this, navAgent, animator, soundPlayer);
        var walkAroundField = new WalkToTargetState(this, navAgent, animator, soundPlayer);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var nextSpotInField = new FindNextSpotInFieldState(this);
        var walkToSpawnPoint = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var dropStuff = new GenericState(tick: DropStuff);
        
        AT(giveUpJob, JobTerminated());

        AT(idle, findFieldCenter, BeenIdleFor(2f));
        AT(findFieldCenter, idle, SearchFailed());
        AT(findFieldCenter, walkToField, HasSomewhereToGo());
        AT(walkToField, harvest, AtTarget(0.25f));
        AT(harvest, nextSpotInField, NoGrassLeft());
        AT(nextSpotInField, walkToDropPoint, FieldSpotNotFound());
        AT(nextSpotInField, walkAroundField, HasSomewhereToGo());
        AT(walkAroundField, harvest, AtTarget(0.25f));
        AT(harvest, walkToDropPoint, FieldIsFinished());
        AT(walkToDropPoint, dropStuff, AtDropPoint());
        AT(dropStuff, walkToSpawnPoint, () => grassHarvested <= 0.1f);
        AT(walkToSpawnPoint, idle, AtSpawnPoint());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job.IsTerminated;
        Func<bool> BeenIdleFor(float t) => () => idle.TimeIdle > t;
        Func<bool> HasSomewhereToGo() => () => !CloseEnoughTo(target, 0.25f);
        Func<bool> SearchFailed() => () => !findFieldCenter.FieldFound && !findFieldCenter.InProgress;
        Func<bool> NoGrassLeft() => () => harvest.NoGrassLeft;
        Func<bool> AtTarget(float d) => () => CloseEnoughTo(target, d);
        Func<bool> FieldSpotNotFound() => () => !nextSpotInField.LocationFound;
        Func<bool> FieldIsFinished() => () => FieldFinished();
        Func<bool> AtDropPoint() => () => CloseEnoughTo(Job.Building.dropPoint, 2f);
        Func<bool> AtSpawnPoint() => () => CloseEnoughTo(Job.Building.spawnPoint, 4f);
    }

    private void DropStuff()
    {
        int qtty = Mathf.CeilToInt(grassHarvested * dropSpec.stackSize / 1000f);
        qtty = Mathf.Min(dropSpec.stackSize, qtty);
        DropController.Instance.Drop(dropSpec, Job.Building.dropPoint.transform.position, qtty);
        Think($"I dropped {grassHarvested} grass as {qtty} wheat");
        grassHarvested = 0f;
    }

    private bool FieldFinished()
    {
        if (grassHarvested >= 1000f)
            return true;

        float ammt = GroundWearManager.Instance.GetAvailableGrass(fieldCenter, fieldRadius * 0.75f);
        return ammt < 10f;
    }
}
