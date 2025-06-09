using UnityEngine;

public class AgingBuffInstance : BuffInstance
{
    private readonly AgingBuffSpec agingBuffSpec;

    public AgingBuffInstance(AgingBuffSpec agingBuffSpec)
    {
        this.agingBuffSpec = agingBuffSpec;
    }

    public override void ApplyTo(AnimalStats stats, float timeDelta)
    {
        stats.age += Time.deltaTime;

        float ageFactor = (stats.age < agingBuffSpec.oldAgeSeconds) ? 0.0f : Mathf.InverseLerp(agingBuffSpec.oldAgeSeconds, agingBuffSpec.lifespanSeconds, stats.age);

        // Juveniles are small
        if (stats.age <= agingBuffSpec.maturityAgeSeconds)
        {
            var x = Mathf.InverseLerp(0f, agingBuffSpec.maturityAgeSeconds, stats.age);
            stats.scale = Mathf.Lerp(agingBuffSpec.juvenileScale, 1f, x);
        }
        else
        {
            stats.scale = 1.0f;
        }

        // Decreasing health due to old age
        float oldAgeHealthDetriment = agingBuffSpec.oldAgeHealthImpactPerSecond * ageFactor * Time.deltaTime;
        if (stats.isSleeping) // Less if sleeping!
            oldAgeHealthDetriment *= agingBuffSpec.sleepingHealthDecreaseMultiplier;
        stats.health -= oldAgeHealthDetriment;
    }
}
