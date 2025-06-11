using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DiseaseBuffInstance : BuffInstance
{
    public float timeRemaining;
    public bool speedUpdated;
    public float originalSpeed;


    private DiseaseBuffSpec DiseaseBuffSpec { get { return (DiseaseBuffSpec)Spec; } }

    public DiseaseBuffInstance(float duration)
    {
        timeRemaining = duration;
    }

    public override IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta)
    {
        Debug.Assert(DiseaseBuffSpec != null, $"Buff spec not set for {this.GetType().Name}");

        if (!DiseaseBuffSpec.isPermanent)
        {
            // Since we might get serialised and rehydrated, we can't store the start time 
            // Need to keep track of time remaining independently
            timeRemaining -= timeDelta;
            if (timeRemaining <= 0)
            {
                // Disease expired.  Reset altered stats
                stats.speed = originalSpeed;

                isExpired = true;
                return Enumerable.Empty<BuffInstance>();
            }
        }

        stats.health = Mathf.Max(0f, stats.health - DiseaseBuffSpec.healthDecreasePerSecond * timeDelta);
        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - DiseaseBuffSpec.foodDecreasePerSecond * timeDelta);

        if (!speedUpdated)
        {
            originalSpeed = stats.speed; // Remember the original value
            stats.speed *= DiseaseBuffSpec.speedMultiplierOneOff;
            speedUpdated = true;
        }

        return Enumerable.Empty<BuffInstance>();
    }
}
