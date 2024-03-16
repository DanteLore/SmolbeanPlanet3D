using System;

public class Builder : SmolbeanColonist
{
    public string buildingLayer = "Buildings";
    public float sleepTime = 5f;

    public BuildingSite TargetBuilding { get; set; }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }

    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);
        var sleep = new ColonistSleepState(this);
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

        StateMachine.SetStartState(idle);

        Func<bool> IdleForAWhile() => () => idle.TimeIdle > 8f;
        Func<bool> SleepingForAWhile() => () => sleep.TimeAsleep > sleepTime;
        Func<bool> CloseEnoughToSite() => () => TargetBuilding != null && CloseEnoughTo(TargetBuilding.GetSpawnPoint());
        Func<bool> TargetFound() => () => TargetBuilding != null;
        Func<bool> TargetNotFound() => () => TargetBuilding == null;
        Func<bool> CloseEnoughToHome() => () => CloseEnoughTo(SpawnPoint);
        Func<bool> BuildingComplete() => () => TargetBuilding.IsComplete;
        Func<bool> IsStuck() => () => walkToSite.StuckTime >= 2f;
    }
}
