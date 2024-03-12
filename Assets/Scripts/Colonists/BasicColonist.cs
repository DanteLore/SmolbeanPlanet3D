using UnityEngine;

public class BasicColonist : SmolbeanColonist
{
    protected override void Start()
    {
        base.Start();

        var freeColonist = new FreeColonistState(this, animator, navAgent, soundPlayer);

        StateMachine.SetState(freeColonist);
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
