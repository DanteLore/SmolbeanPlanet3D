using UnityEngine;

public class Druid : SmolbeanColonist
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

        //AT(idle, searchForDeliveryJob, HasBeenIdleFor(idleTime));

        StateMachine.SetStartState(idle);

        //Func<bool> JobTerminated() => () => Job == null || Job.IsTerminated;
    }
}
