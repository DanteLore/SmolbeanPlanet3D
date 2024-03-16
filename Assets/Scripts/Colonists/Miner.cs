using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Miner : SmolbeanColonist, IReturnDrops
{
    public float idleTime = 1f;
    public float mineCooldown = 1f;
    public int maxStacks = 3;
    public float sleepTime = 5.0f;

    public Vector3 DropPoint
    {
        get
        {
            return Home.GetDropPoint();
        }
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        var giveUpJob = new SwitchColonistToFreeState(this);

        var idle = new IdleState(animator);
        var getReady = new MinerBlinkInTheSunlightState(this);
        var operateMine = new OperateMineState(this, soundPlayer);
        var walkToDropPoint = new WalkToDropPointState(this, navAgent, animator, soundPlayer);
        var dropInventory = new DropInventoryAtDropPointState(this, DropController.Instance);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);

        AT(giveUpJob, JobTerminated());

        AT(idle, operateMine, ReadyToGo());
        AT(operateMine, getReady, MiningFinished());

        AT(getReady, walkToDropPoint, InventoryIsNotEmpty());
        AT(getReady, idle, InventoryIsEmpty());

        AT(walkToDropPoint, dropInventory, IsAtDropPoint());
        AT(dropInventory, walkHome, InventoryIsEmpty());
        AT(walkHome, idle, IsAtSpawnPoint());

        StateMachine.SetStartState(getReady);

        Func<bool> JobTerminated() => () => job.IsTerminated;
        Func<bool> IsAtSpawnPoint() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> IsAtDropPoint() => () => CloseEnoughTo(DropPoint);
        Func<bool> InventoryIsEmpty() => Inventory.IsEmpty;
        Func<bool> InventoryIsNotEmpty() => () => !Inventory.IsEmpty();
        Func<bool> MiningFinished() => () => operateMine.Finished;
        Func<bool> ReadyToGo() => () => idle.TimeIdle >= idleTime && Home.DropPointContents().Where(s => s.IsFull()).Count() < maxStacks;
    }
}