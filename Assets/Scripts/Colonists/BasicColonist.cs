using UnityEngine;

public class BasicColonist : SmolbeanColonist
{
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
                name = DodoNameGenerator.Generate(), // TODO:  People are not dodos!
                age = 0f, // TODO: Randomise starting age!
                health = species.initialHealth,
                foodLevel = Random.Range(species.initialFoodLevelMin, species.initialFoodLevelMax)
            };
        }
    }
}
