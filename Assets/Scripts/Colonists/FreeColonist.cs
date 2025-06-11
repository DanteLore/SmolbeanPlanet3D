using System;
using System.Linq;
using Random = UnityEngine.Random;

public class FreeColonist : SmolbeanColonist
{
    protected override void Start()
    {
        base.Start();

        var idle = new IdleState(animator);
        var sleep = new SleepState(this, animator, soundPlayer);
        var beFree = new FreeColonistState(this, animator, navAgent, soundPlayer);
        var findJob = new SearchForJobState(this, JobController.Instance);
        var doJob = new DoJobState(this, animator, navAgent, soundPlayer);

        AT(sleep, IsNight());
        AT(doJob, HasJob());

        AT(sleep, idle, WakeupTime());

        AT(idle, beFree, NoJob());
        AT(findJob, beFree, NoJob());

        AT(beFree, findJob, JobAvailable());

        StateMachine.SetStartState(beFree);

        Func<bool> JobAvailable() => () => Job == null && JobController.Instance.Vacancies.Any();
        Func<bool> IsNight() => () => DayNightCycleController.Instance.TimeIsBetween(23f, 5f);
        Func<bool> WakeupTime() => () => DayNightCycleController.Instance.TimeIsBetween(5f, 23f);
        Func<bool> HasJob() => () => Job != null;
        Func<bool> NoJob() => () => Job == null;
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        if (newStats != null)
        {
            stats = newStats;
        }
        else
        {
            stats = new ColonistStats()
            {
                name = ColonistNameGenerator.Generate(), 
                age = Random.Range(Species.maturityAgeSeconds, Species.oldAgeSeconds),
                health = Species.initialHealth,
                foodLevel = Random.Range(Species.initialFoodLevelMin, Species.initialFoodLevelMax),
                scale = 1.0f,
                speed = Random.Range(Species.minSpeed, Species.maxSpeed)
            };
        }
    }
}
