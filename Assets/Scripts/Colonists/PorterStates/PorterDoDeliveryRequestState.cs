using UnityEngine;
using UnityEngine.AI;
using System;

public class PorterDoDeliveryRequestState : IState
{
    private Animator animator;
    private Porter porter;
    private SoundPlayer soundPlayer;
    private StateMachine stateMachine;
    private IState startState;
    private DeliveryManager deliveryManager;    
    
    public bool Finished { get; private set; }

    public PorterDoDeliveryRequestState(Porter porter, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer, DeliveryManager deliveryManager)
    {
        this.porter = porter;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
        this.deliveryManager = deliveryManager;

        stateMachine = new StateMachine(shouldLog:false);

        var walkToDestination = new PorterWalkToBuildingState(porter, navAgent, animator, soundPlayer);
        var succeeded = new PorterFinishedDeliveryRequestState(this, porter, DeliveryManager.Instance);
        var failed = new PorterFailedDeliveryRequestState(this, porter, DeliveryManager.Instance);
        var goToStorehouse = new WalkHomeState(porter, navAgent, animator, soundPlayer);
        var pickupStuff = new PorterLoadDeliveryItemsState(porter);
        var unloadStuff = new PorterUnloadDeliveryItemsState(porter);
        var putStuffBack = new PorterStoreDropsState(porter, DropController.Instance);

        AT(goToStorehouse, pickupStuff, IsAtStorehouseAndReadyToPickup());
        AT(goToStorehouse, putStuffBack, IsAtStorehouseAndReadyToDropOff());
        AT(putStuffBack, failed, InventoryEmpty());
        AT(pickupStuff, walkToDestination, HasStuffInInventory());
        AT(walkToDestination, unloadStuff, IsAtDestinationBuilding());
        AT(unloadStuff, succeeded, DeliveryIsFinished());

        AT(pickupStuff, failed, DidNotGetTheStuff());
        AT(pickupStuff, failed, DeliveryRequestCancelled());
        
        AT(walkToDestination, goToStorehouse, DeliveryRequestCancelled());
        AT(unloadStuff, goToStorehouse, DeliveryRequestCancelled());

        // Handle deletion of destination building
        AT(walkToDestination, goToStorehouse, BuildingDeleted());

        startState = goToStorehouse;

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        Func<bool> IsAtDestinationBuilding() => () => porter.DeliveryRequest.Building != null && porter.CloseEnoughTo(porter.DeliveryRequest.Building.GetSpawnPoint());
        Func<bool> IsAtStorehouseAndReadyToPickup() => () => porter.CloseEnoughTo(porter.SpawnPoint) && porter.DeliveryRequest != null && porter.DeliveryRequest.IsComplete == false;
        Func<bool> IsAtStorehouseAndReadyToDropOff() => () => porter.CloseEnoughTo(porter.SpawnPoint) && (porter.DeliveryRequest == null || porter.DeliveryRequest.IsComplete);
        Func<bool> HasStuffInInventory() => () => porter.Inventory.Contains(porter.DeliveryRequest.Item, porter.DeliveryRequest.Quantity);
        Func<bool> DidNotGetTheStuff() => () => !porter.Inventory.Contains(porter.DeliveryRequest.Item, porter.DeliveryRequest.Quantity);
        Func<bool> DeliveryIsFinished() => () => porter.DeliveryRequest == null || porter.DeliveryRequest.IsComplete;
        Func<bool> DeliveryRequestCancelled() => () => DeliveryIsFinished().Invoke() && HasStuffInInventory().Invoke();
        Func<bool> InventoryEmpty() => () => porter.Inventory.IsEmpty();
        Func<bool> BuildingDeleted() => () => porter.DeliveryRequest == null || porter.DeliveryRequest.Building == null;
    }

    public void SetFinished(bool val)
    {
        Finished = val;
    }

    public void OnEnter()
    {
        Finished = false;

        stateMachine.SetState(startState);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        stateMachine?.Tick();
    }
}