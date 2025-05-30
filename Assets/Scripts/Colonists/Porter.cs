using System;
using UnityEngine;

public class Porter : SmolbeanColonist, IGatherDrops, IDeliverDrops
{    
    public float idleTime = 10f;

    public GameObject TargetDrop { get; set; }

    public DeliveryRequest DeliveryRequest { get; set; }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        //StateMachine.ShouldLog = true;
        //StateMachine.OnLogMessage += message => Think(message);

        var gridManager = FindFirstObjectByType<GridManager>();

        var idle = new IdleState(animator);
        var giveUpJob = new SwitchColonistToFreeState(this);
        var searchForDeliveryJob = new PorterClaimDeliveryRequest(this, DeliveryManager.Instance);
        var searchForCollectionJob = new PorterSearchForDropToCollectState(this, dropLayer, gridManager);
        var doDelivery = new PorterDoDeliveryRequestState(this, animator, navAgent, soundPlayer, DeliveryManager.Instance);
        var fetchDrop = new PorterFetchDropsState(this, animator, navAgent, soundPlayer, dropLayer);

        AT(giveUpJob, JobTerminated());

        AT(searchForDeliveryJob, doDelivery, DeliveryAssigned());
        AT(searchForDeliveryJob, searchForCollectionJob, NoDeliveryToDo());
        AT(doDelivery, idle, DeliveryComplete());
        
        AT(searchForCollectionJob, fetchDrop, DropFound());
        AT(searchForCollectionJob, idle, NoDropFound());
        AT(fetchDrop, idle, FetchDropSucceeded());
        AT(fetchDrop, searchForDeliveryJob, FetchDropFailed());

        AT(idle, searchForDeliveryJob, HasBeenIdleFor(idleTime));

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job == null || Job.IsTerminated;
        Func<bool> DropFound() => () => TargetDrop != null;
        Func<bool> NoDropFound() => () => TargetDrop == null && !searchForCollectionJob.InProgress;
        Func<bool> HasBeenIdleFor(float t) => () => idle.TimeIdle >= t;
        Func<bool> FetchDropSucceeded() => () => fetchDrop.Finished && CloseEnoughToBuildingSpawnPoint(Job.Building);
        Func<bool> FetchDropFailed() => () => fetchDrop.Finished && !CloseEnoughToBuildingSpawnPoint(Job.Building);
        Func<bool> NoDeliveryToDo() => () => DeliveryRequest == null;
        Func<bool> DeliveryAssigned() => () => DeliveryRequest != null;
        Func<bool> DeliveryComplete() => () => doDelivery.Finished;
    }

    private bool CloseEnoughToBuildingSpawnPoint(SmolbeanBuilding building)
    {
        return CloseEnoughTo(building.GetSpawnPoint(), 1f) ||
                CloseEnoughTo(building.gameObject, 2f);
    }
}
