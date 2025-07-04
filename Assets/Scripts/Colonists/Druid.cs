using System;
using UnityEngine;

public class Druid : SmolbeanColonist
{    
    public float idleTime = 10f;

    protected StoneCircle StoneCircle
    {
        get
        {
            return (StoneCircle)Job.Building;
        }
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        //StateMachine.ShouldLog = true;
        //StateMachine.OnLogMessage += message => Think(message);

        var giveUpJob = new SwitchColonistToFreeState(this);
        var idle = new IdleState(animator);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);
        var doJob = new DruidDoOfferingState(this, soundPlayer);

        AT(giveUpJob, JobTerminated());

        AT(idle, doJob, StoneCircleReady());
        AT(doJob, idle, JobDone());
        AT(walkHome, idle, AtSpawnPoint());

        StateMachine.SetStartState(idle);

        Func<bool> JobTerminated() => () => Job.IsTerminated;
        Func<bool> AtSpawnPoint() => () => CloseEnoughTo(Job.Building.spawnPoint, 0.5f);
        Func<bool> StoneCircleReady() =>() => StoneCircle.IsReadyToStart;
        Func<bool> JobDone() => () => StoneCircle.IsFinished;
    }
}
