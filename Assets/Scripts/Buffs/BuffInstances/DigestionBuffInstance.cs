using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DigestionBuffInstance : BuffInstance
{
    public override void ApplyTo(AnimalStats stats, AnimalSpec species, float timeDelta, List<BuffInstance> newBuffs)
    {
        var digestionBuffSpec = (DigestionBuffSpec)Spec; // Cache for speed

        float foodDelta = digestionBuffSpec.foodDigestedPerSecond * timeDelta;

        if (stats.isSleeping) // Less if sleeping!
            foodDelta *= digestionBuffSpec.sleepingDigestionMultiplier;

        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - foodDelta);

        if (stats.foodLevel <= digestionBuffSpec.starvationThreshold)
        {
            // Health decrease due to starvation
            float healthDelta = digestionBuffSpec.starvationRatePerSecond * timeDelta;
            healthDelta *= 1f - Mathf.InverseLerp(0f, b: digestionBuffSpec.starvationThreshold, stats.foodLevel);
            if (stats.isSleeping) // Less if sleeping!
                healthDelta *= digestionBuffSpec.sleepingHealthDecreaseMultiplier;
            stats.health -= healthDelta;
        }
        else
        {
            // Health recover with a full stomach
            float healthDelta = digestionBuffSpec.healthRecoveryPerSecond * timeDelta;
            healthDelta *= Mathf.InverseLerp(digestionBuffSpec.starvationThreshold, digestionBuffSpec.maxFoodLevel, stats.foodLevel);
            stats.health = Mathf.Min(digestionBuffSpec.maxHealth, stats.health + healthDelta);
        }
    }
}
