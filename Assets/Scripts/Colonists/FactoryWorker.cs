using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorker : Colonist, IReturnDrops
{
    public float idleTime = 1f;

    public Vector3 DropPoint
    {
        get
        {
            return Home.GetDropPoint();
        }
    }
    
    private StateMachine stateMachine;

    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine(shouldLog:true);

        var factory = (FactoryBuilding)this.Home;

        var idle = new IdleState(animator);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var pickupIngredients = new FactoryWorkerPickupIngredientsState(factory);
        var doJob = new FactoryWorkerDoJobState(this, factory, soundPlayer, DropController.Instance);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);

        AT(idle, pickupIngredients, IngredientsPresent());
        AT(pickupIngredients, doJob, FactoryReady());
        AT(doJob, walkToDropPoint, JobDone());
        AT(walkToDropPoint, dropInventory, AtDropPoint());
        AT(dropInventory, walkHome, InventoryEmpty());
        AT(walkHome, idle, AtSpawnPoint());

        stateMachine.SetState(idle);

        Func<bool> AtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> AtDropPoint() => () => CloseEnoughTo(DropPoint);
        Func<bool> IngredientsPresent() => factory.HasResources;
        Func<bool> InventoryEmpty() => Inventory.IsEmpty;
        Func<bool> FactoryReady() =>() => factory.IsReadyToStart;
        Func<bool> JobDone() => () => factory.IsFinished;

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
