using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DigestionBuffInstance : BuffInstance
{
    private DigestionBuffSpec DigestionBuffSpec { get { return (DigestionBuffSpec)Spec; }}

    public override IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta)
    {
        Debug.Assert(DigestionBuffSpec != null, $"Buff spec not set for {this.GetType().Name}");

        float foodDelta = DigestionBuffSpec.foodDigestedPerSecond * Time.deltaTime;

        if (stats.isSleeping) // Less if sleeping!
            foodDelta *= DigestionBuffSpec.sleepingDigestionMultiplier;

        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - foodDelta);

        if (stats.foodLevel <= DigestionBuffSpec.starvationThreshold)
        {
            // Health decrease due to starvation
            float healthDelta = DigestionBuffSpec.starvationRatePerSecond * Time.deltaTime;
            healthDelta *= 1f - Mathf.InverseLerp(0f, b: DigestionBuffSpec.starvationThreshold, stats.foodLevel);
            if (stats.isSleeping) // Less if sleeping!
                healthDelta *= DigestionBuffSpec.sleepingHealthDecreaseMultiplier;
            stats.health -= healthDelta;
        }
        else
        {
            // Health recover with a full stomach
            float healthDelta = DigestionBuffSpec.healthRecoveryPerSecond * Time.deltaTime;
            healthDelta *= Mathf.InverseLerp(DigestionBuffSpec.starvationThreshold, DigestionBuffSpec.maxFoodLevel, stats.foodLevel);
            stats.health = Mathf.Min(DigestionBuffSpec.maxHealth, stats.health + healthDelta);
        }

        return Enumerable.Empty<BuffInstance>();
    }
}
