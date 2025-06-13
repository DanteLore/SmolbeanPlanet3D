using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgingBuffInstance : BuffInstance
{
    public override void ApplyTo(AnimalStats stats, AnimalSpec species, float timeDelta, List<BuffInstance> newBuffs)
    {
        var agingBuffSpec = (AgingBuffSpec)Spec; // Cache for speed

        stats.age += timeDelta;
        stats.timeOnIsland += timeDelta;

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
        float oldAgeHealthDetriment = agingBuffSpec.oldAgeHealthImpactPerSecond * ageFactor * timeDelta;
        if (stats.isSleeping) // Less if sleeping!
            oldAgeHealthDetriment *= agingBuffSpec.sleepingHealthDecreaseMultiplier;
        stats.health -= oldAgeHealthDetriment;

        // Catch a disease or two
        foreach (var ds in agingBuffSpec.diseases)
        {
            if (ds.CheckStart(stats, species, timeDelta))
                newBuffs.Add(ds.GetBuff());
        }
    }
}
