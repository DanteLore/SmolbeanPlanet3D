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

        AT(sleep, idle, WakeupTime());

        AT(idle, doJob, HasJob());
        AT(idle, beFree, NoJob());

        AT(findJob, doJob, HasJob());
        AT(findJob, beFree, NoJob());

        AT(beFree, findJob, JobAvailable());

        StateMachine.SetStartState(beFree);

        Func<bool> JobAvailable() => () => job == null && JobController.Instance.Vacancies.Any();
        Func<bool> IsNight() => () => DayNightCycleController.Instance.TimeIsBetween(21f, 5f);
        Func<bool> WakeupTime() => () => DayNightCycleController.Instance.TimeIsBetween(5f, 21f);
        Func<bool> HasJob() => () => job != null;
        Func<bool> NoJob() => () => job == null;
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
                age = Random.Range(species.maturityAgeSeconds, species.oldAgeSeconds),
                health = species.initialHealth,
                foodLevel = Random.Range(species.initialFoodLevelMin, species.initialFoodLevelMax)
            };
        }
    }
}
