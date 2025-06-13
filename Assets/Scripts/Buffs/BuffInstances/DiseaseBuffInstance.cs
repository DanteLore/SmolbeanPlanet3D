using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DiseaseBuffInstance : BuffInstance
{
    public float timeRemaining;
    public bool speedUpdated;

    public DiseaseBuffInstance(float duration)
    {
        timeRemaining = duration;
    }

    public override void ApplyTo(AnimalStats stats, AnimalSpec species, float timeDelta, List<BuffInstance> newBuffs)
    {
        var diseaseBuffSpec = (DiseaseBuffSpec)Spec;

        if (!diseaseBuffSpec.isPermanent)
        {
            // Since we might get serialised and rehydrated, we can't store the start time 
            // Need to keep track of time remaining independently
            timeRemaining -= timeDelta;
            if (timeRemaining <= 0)
            {
                // Disease expired.  Reset altered stats
                stats.speed /= diseaseBuffSpec.speedMultiplierOneOff;

                isExpired = true;
                return;
            }
        }

        stats.health = Mathf.Max(0f, stats.health - diseaseBuffSpec.healthDecreasePerSecond * timeDelta);
        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - diseaseBuffSpec.foodDecreasePerSecond * timeDelta);

        if (!speedUpdated)
        {
            stats.speed *= diseaseBuffSpec.speedMultiplierOneOff;
            speedUpdated = true;
        }
    }
}
