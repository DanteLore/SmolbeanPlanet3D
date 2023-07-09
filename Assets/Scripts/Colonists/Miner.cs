using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Miner : Colonist, IReturnDrops
{
    public float idleTime = 1f;
    public float mineCooldown = 1f;

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

        stateMachine = new StateMachine(shouldLog:false);

        var idle = new IdleState(animator);
        var getReady = new MinerBlinkInTheSunlightState(this);
        var operateMine = new OperateMineState(this, animator, soundPlayer);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);

        AT(idle, operateMine, HasBeenIdleForAWhile());
        AT(operateMine, getReady, MiningFinished());

        AT(getReady, walkToDropPoint, InventoryIsNotEmpty());
        AT(getReady, idle, InventoryIsEmpty());

        AT(walkToDropPoint, dropInventory, IsAtDropPoint());
        AT(dropInventory, walkHome, InventoryIsEmpty());
        AT(walkHome, idle, IsAtSpawnPoint());

        stateMachine.SetState(getReady);

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> IsAtDropPoint() => () => CloseEnoughTo(DropPoint);
        Func<bool> InventoryIsEmpty() => Inventory.IsEmpty;
        Func<bool> InventoryIsNotEmpty() => () => !Inventory.IsEmpty();
        Func<bool> HasBeenIdleForAWhile() => () => idle.TimeIdle >= idleTime;
        Func<bool> MiningFinished() => () => operateMine.Finished;
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
