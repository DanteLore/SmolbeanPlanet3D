using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgingBuffInstance : BuffInstance
{
    private AgingBuffSpec AgingBuffSpec { get { return (AgingBuffSpec)Spec; }}

    public override IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta)
    {
        Debug.Assert(AgingBuffSpec != null, $"Buff spec not set for {this.GetType().Name}");

        stats.age += Time.deltaTime;

        float ageFactor = (stats.age < AgingBuffSpec.oldAgeSeconds) ? 0.0f : Mathf.InverseLerp(AgingBuffSpec.oldAgeSeconds, AgingBuffSpec.lifespanSeconds, stats.age);

        // Juveniles are small
        if (stats.age <= AgingBuffSpec.maturityAgeSeconds)
        {
            var x = Mathf.InverseLerp(0f, AgingBuffSpec.maturityAgeSeconds, stats.age);
            stats.scale = Mathf.Lerp(AgingBuffSpec.juvenileScale, 1f, x);
        }
        else
        {
            stats.scale = 1.0f;
        }

        // Decreasing health due to old age
        float oldAgeHealthDetriment = AgingBuffSpec.oldAgeHealthImpactPerSecond * ageFactor * Time.deltaTime;
        if (stats.isSleeping) // Less if sleeping!
            oldAgeHealthDetriment *= AgingBuffSpec.sleepingHealthDecreaseMultiplier;
        stats.health -= oldAgeHealthDetriment;

        // Catch a disease or two
        var newBuffs = new List<BuffInstance>();
        foreach (var ds in AgingBuffSpec.diseases)
        {
            float p = 1 / ds.probabilitySeconds * timeDelta;
            if (stats.age > AgingBuffSpec.oldAgeSeconds)
                p *= stats.oldAgeDiseaseChanceMultiplier;

            if (Random.Range(0.0f, 1.0f) < p)
                newBuffs.Add(ds.GetBuff());
        }

        return newBuffs;
    }
}
