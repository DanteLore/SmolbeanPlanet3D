using System;
using System.Linq;

public abstract class FactoryWorker : SmolbeanColonist, IReturnDrops
{
    public float idleTime = 1f;
    public int maxStacks = 5;

    protected FactoryBuilding Factory
    {
        get
        {
            return (FactoryBuilding)Job.Building;
        }
    }

    protected override void Start()
    {
        base.Start();

        var giveUpJob = new SwitchColonistToFreeState(this);
        var idle = new IdleState(animator);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var pickupIngredients = new FactoryWorkerPickupIngredientsState(this);
        var doJob = new FactoryWorkerDoJobState(this, soundPlayer, DropController.Instance);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);

        AT(giveUpJob, JobTerminated());

        AT(idle, pickupIngredients, ReadyToMake());
        AT(pickupIngredients, doJob, FactoryReady());
        AT(doJob, walkToDropPoint, JobDone());
        AT(walkToDropPoint, dropInventory, AtDropPoint());
        AT(dropInventory, walkHome, InventoryEmpty());
        AT(walkHome, idle, AtSpawnPoint());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job.IsTerminated;
        Func<bool> AtSpawnPoint() => () => CloseEnoughTo(Job.Building.spawnPoint);
        Func<bool> AtDropPoint() => () => CloseEnoughTo(Job.Building.dropPoint);
        Func<bool> ReadyToMake() => () => Factory.HasResources() && Job.Building.DropPointContents().Where(s => s.IsFull()).Count() < maxStacks;
        Func<bool> InventoryEmpty() => Inventory.IsEmpty;
        Func<bool> FactoryReady() =>() => Factory.IsReadyToStart;
        Func<bool> JobDone() => () => Factory.IsFinished;
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }
}
