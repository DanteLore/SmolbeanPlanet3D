using System;
using System.Linq;
using UnityEngine;

public abstract class FactoryWorker : SmolbeanColonist, IReturnDrops
{
    public float idleTime = 1f;
    public int maxStacks = 5;

    public Vector3 DropPoint
    {
        get
        {
            return Home.GetDropPoint();
        }
    }

    protected override void Start()
    {
        base.Start();

        var factory = (FactoryBuilding)Home;

        var idle = new IdleState(animator);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var pickupIngredients = new FactoryWorkerPickupIngredientsState(factory);
        var doJob = new FactoryWorkerDoJobState(this, factory, soundPlayer, DropController.Instance);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);

        AT(idle, pickupIngredients, ReadyToMake());
        AT(pickupIngredients, doJob, FactoryReady());
        AT(doJob, walkToDropPoint, JobDone());
        AT(walkToDropPoint, dropInventory, AtDropPoint());
        AT(dropInventory, walkHome, InventoryEmpty());
        AT(walkHome, idle, AtSpawnPoint());

        StateMachine.SetStartState(idle);

        Func<bool> AtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> AtDropPoint() => () => CloseEnoughTo(DropPoint);
        Func<bool> ReadyToMake() => () => factory.HasResources() && Home.DropPointContents().Where(s => s.IsFull()).Count() < maxStacks;
        Func<bool> InventoryEmpty() => Inventory.IsEmpty;
        Func<bool> FactoryReady() =>() => factory.IsReadyToStart;
        Func<bool> JobDone() => () => factory.IsFinished;
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }
}
