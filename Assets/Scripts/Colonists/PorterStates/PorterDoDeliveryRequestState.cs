using UnityEngine;
using UnityEngine.AI;
using System;

public class PorterDoDeliveryRequestState : CompoundState
{
    private readonly Porter porter;
    private readonly DeliveryManager deliveryManager;

    public PorterDoDeliveryRequestState(Porter porter, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer, DeliveryManager deliveryManager)
        : base()
    {
        this.porter = porter;
        this.deliveryManager = deliveryManager;

        stateMachine.ShouldLog = true;
        stateMachine.OnLogMessage += message => porter.Think(message);

        var walkToDestination = new PorterWalkToBuildingState(porter, navAgent, animator, soundPlayer);
        var succeeded = new PorterFinishedDeliveryRequestState(this, porter, deliveryManager);
        var failed = new PorterFailedDeliveryRequestState(this, porter, deliveryManager);
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
        AT(goToStorehouse, succeeded, BuildingDeleted());

        stateMachine.SetStartState(goToStorehouse);

        Func<bool> IsAtDestinationBuilding() => () => porter.DeliveryRequest.Building != null && CloseEnoughToBuildingSpawnPoint(porter.DeliveryRequest.Building);
        Func<bool> IsAtStorehouseAndReadyToPickup() => () => CloseEnoughToBuildingSpawnPoint(porter.Job.Building) && porter.DeliveryRequest != null && porter.DeliveryRequest.IsComplete == false;
        Func<bool> IsAtStorehouseAndReadyToDropOff() => () => CloseEnoughToBuildingSpawnPoint(porter.Job.Building) && (porter.DeliveryRequest == null || porter.DeliveryRequest.IsComplete);
        Func<bool> HasStuffInInventory() => () => porter.Inventory.Contains(porter.DeliveryRequest.Item, porter.DeliveryRequest.Quantity);
        Func<bool> DidNotGetTheStuff() => () => !porter.Inventory.Contains(porter.DeliveryRequest.Item, porter.DeliveryRequest.Quantity);
        Func<bool> DeliveryIsFinished() => () => porter.DeliveryRequest == null || porter.DeliveryRequest.IsComplete;
        Func<bool> DeliveryRequestCancelled() => () => DeliveryIsFinished().Invoke() && HasStuffInInventory().Invoke();
        Func<bool> InventoryEmpty() => () => porter.Inventory.IsEmpty();
        Func<bool> BuildingDeleted() => () => porter.DeliveryRequest == null || porter.DeliveryRequest.Building == null;
    }

    private bool CloseEnoughToBuildingSpawnPoint(SmolbeanBuilding building)
    {
        return porter.CloseEnoughTo(building.GetSpawnPoint(), 2f) ||
                porter.CloseEnoughTo(building.gameObject, 3f);
    }

    public override void OnExit()
    {
        if(porter.DeliveryRequest != null && !porter.DeliveryRequest.IsComplete)
        {
            deliveryManager.ReturnDelivery(porter, porter.DeliveryRequest);
            porter.DeliveryRequest = null;
        }

        base.OnExit();
    }
}
