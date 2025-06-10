using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgingBuffInstance : BuffInstance
{
    private readonly AgingBuffSpec agingBuffSpec;

    public AgingBuffInstance(AgingBuffSpec agingBuffSpec)
    {
        this.agingBuffSpec = agingBuffSpec;
    }

    public override IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta)
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

        // Catch a disease or two
        var newBuffs = new List<BuffInstance>();
        foreach (var ds in agingBuffSpec.diseases)
        {
            float p = 1 / ds.probabilitySeconds * timeDelta;
            if (stats.age > agingBuffSpec.oldAgeSeconds)
                p *= stats.oldAgeDiseaseChanceMultiplier;

            if (Random.Range(0.0f, 1.0f) < p)
                newBuffs.Add(ds.GetBuff());
        }

        return newBuffs;
    }

    protected override BuffSpec GetBuffSpec()
    {
        return agingBuffSpec;
    }
}
