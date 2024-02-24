using System;
using UnityEngine;

public class Porter : SmolbeanColonist, IGatherDrops, IDeliverDrops
{    
    public float idleTime = 1f;
    public float sleepTime = 2f;

    public GameObject TargetDrop { get; set; }

    public DeliveryRequest DeliveryRequest { get; set; }

    private StateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine(shouldLog:false);

        var idle = new IdleState(animator);

        var searchForDeliveryJob = new PorterClaimDeliveryRequest(this, DeliveryManager.Instance);
        var searchForCollectionJob = new PorterSearchForDropToCollectState(this, dropLayer);
        
        var doDelivery = new PorterDoDeliveryRequestState(this, animator, navAgent, soundPlayer, DeliveryManager.Instance);
        var fetchDrop = new PorterFetchDropsState(this, animator, navAgent, soundPlayer, dropLayer);
        
        AT(searchForDeliveryJob, doDelivery, DeliveryAssigned());
        AT(searchForDeliveryJob, searchForCollectionJob, NoDeliveryToDo());
        AT(doDelivery, idle, DeliveryComplete());
        
        AT(searchForCollectionJob, fetchDrop, DropFound());
        AT(searchForCollectionJob, idle, NoDropFound());
        AT(fetchDrop, idle, FetchDropSucceeded());
        AT(fetchDrop, searchForDeliveryJob, FetchDropFailed());

        AT(idle, searchForDeliveryJob, HasBeenIdleForAWhile());

        stateMachine.SetState(idle);

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        Func<bool> DropFound() => () => TargetDrop != null;
        Func<bool> NoDropFound() => () => TargetDrop == null;
        Func<bool> HasBeenIdleForAWhile() => () => idle.TimeIdle >= idleTime;
        Func<bool> FetchDropSucceeded() => () => fetchDrop.Finished && CloseEnoughTo(SpawnPoint);
        Func<bool> FetchDropFailed() => () => fetchDrop.Finished && !CloseEnoughTo(SpawnPoint);
        Func<bool> NoDeliveryToDo() => () => DeliveryRequest == null;
        Func<bool> DeliveryAssigned() => () => DeliveryRequest != null;
        Func<bool> DeliveryComplete() => () => doDelivery.Finished;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
