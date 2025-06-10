using UnityEngine;

public class DigestionBuffInstance : BuffInstance
{
    private readonly DigestionBuffSpec digestionBuffSpec;

    public DigestionBuffInstance(DigestionBuffSpec digestionBuffSpec)
    {
        this.digestionBuffSpec = digestionBuffSpec;
    }

    public override void ApplyTo(AnimalStats stats, float timeDelta)
    {
        float foodDelta = digestionBuffSpec.foodDigestedPerSecond * Time.deltaTime;

        if (stats.isSleeping) // Less if sleeping!
            foodDelta *= digestionBuffSpec.sleepingDigestionMultiplier;

        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - foodDelta);

        if (stats.foodLevel <= digestionBuffSpec.starvationThreshold)
        {
            // Health decrease due to starvation
            float healthDelta = digestionBuffSpec.starvationRatePerSecond * Time.deltaTime;
            healthDelta *= 1f - Mathf.InverseLerp(0f, b: digestionBuffSpec.starvationThreshold, stats.foodLevel);
            if (stats.isSleeping) // Less if sleeping!
                healthDelta *= digestionBuffSpec.sleepingHealthDecreaseMultiplier;
            stats.health -= healthDelta; 
        }
        else
        {
            // Health recover with a full stomach
            float healthDelta = digestionBuffSpec.healthRecoveryPerSecond * Time.deltaTime;
            healthDelta *= Mathf.InverseLerp(digestionBuffSpec.starvationThreshold, digestionBuffSpec.maxFoodLevel, stats.foodLevel);
            stats.health = Mathf.Min(digestionBuffSpec.maxHealth, stats.health + healthDelta);
        }
    }

    protected override BuffSpec GetBuffSpec()
    {
        return digestionBuffSpec;
    }
}
