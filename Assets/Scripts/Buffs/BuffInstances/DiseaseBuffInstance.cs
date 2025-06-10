using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiseaseBuffInstance : BuffInstance
{
    private readonly DiseaseBuffSpec diseaseBuffSpec;
    private readonly float duration;
    private float startTime;

    public DiseaseBuffInstance(DiseaseBuffSpec diseaseBuffSpec, float duration)
    {
        this.diseaseBuffSpec = diseaseBuffSpec;
        this.duration = duration;
        startTime = Time.time;
    }

    public override IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta)
    {
        if (Time.time >= startTime + duration)
        {
            isExpired = true;
            return Enumerable.Empty<BuffInstance>();
        }

        stats.health = Mathf.Max(0f, stats.health - diseaseBuffSpec.healthDecreasePerSecond * timeDelta);
        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - diseaseBuffSpec.foodDecreasePerSecond * timeDelta);

        return Enumerable.Empty<BuffInstance>();
    }

    protected override BuffSpec GetBuffSpec()
    {
        return diseaseBuffSpec;
    }
}
