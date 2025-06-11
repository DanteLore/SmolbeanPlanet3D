using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DiseaseBuffInstance : BuffInstance
{
    public float timeRemaining;

    private DiseaseBuffSpec DiseaseBuffSpec { get { return (DiseaseBuffSpec)Spec; } }

    public DiseaseBuffInstance(float duration)
    {
        timeRemaining = duration;
    }

    public override IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta)
    {
        Debug.Assert(DiseaseBuffSpec != null, $"Buff spec not set for {this.GetType().Name}");

        // Since we might get serialised and rehydrated, we can't store the start time 
        // Need to keep track of time remaining independently
        timeRemaining -= timeDelta;
        if (timeRemaining <= 0)
        {
            isExpired = true;
            return Enumerable.Empty<BuffInstance>();
        }

        stats.health = Mathf.Max(0f, stats.health - DiseaseBuffSpec.healthDecreasePerSecond * timeDelta);
        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - DiseaseBuffSpec.foodDecreasePerSecond * timeDelta);

        return Enumerable.Empty<BuffInstance>();
    }
}
