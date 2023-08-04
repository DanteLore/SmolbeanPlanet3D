using System;

public class Builder : Colonist
{
    private StateMachine stateMachine;

    public string buildingLayer = "Buildings";
    public float sleepTime = 5f;

    public BuildingSite TargetBuilding { get; set; }

    protected override void Start()
    {
        base.Start();

        stateMachine = new StateMachine(shouldLog:false);

        var idle = new IdleState(animator);
        var sleep = new SleepState(this);
        var chooseBuildingSite = new SearchForBuildingSiteState(this, buildingLayer);
        var walkToSite = new BuilderWalkToBuildingState(this, navAgent, animator, soundPlayer);
        var buildBuilding = new BuildBuildingState(this, soundPlayer);
        var walkHome = new WalkHomeState(this, navAgent, animator, soundPlayer);

        AT(idle, chooseBuildingSite, IdleForAWhile());

        AT(chooseBuildingSite, sleep, TargetNotFound());
        AT(sleep, idle, SleepingForAWhile());

        AT(chooseBuildingSite, walkToSite, TargetFound());
        AT(walkToSite, buildBuilding, CloseEnoughToSite());
        AT(buildBuilding, walkHome, BuildingComplete());
        AT(walkHome, idle, CloseEnoughToHome());

        AT(walkToSite, walkHome, IsStuck());
        AT(walkToSite, walkHome, TargetNotFound());

        AT(buildBuilding, walkHome, TargetNotFound());

        stateMachine.SetState(idle);

        Func<bool> IdleForAWhile() => () => idle.TimeIdle > 8f;
        Func<bool> SleepingForAWhile() => () => sleep.TimeAsleep > sleepTime;
        Func<bool> CloseEnoughToSite() => () => TargetBuilding != null && CloseEnoughTo(TargetBuilding.GetSpawnPoint());
        Func<bool> TargetFound() => () => TargetBuilding != null;
        Func<bool> TargetNotFound() => () => TargetBuilding == null;
        Func<bool> CloseEnoughToHome() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> BuildingComplete() => () => TargetBuilding.IsComplete;
        Func<bool> IsStuck() => () => walkToSite.StuckTime >= 2f;

        void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }
}
