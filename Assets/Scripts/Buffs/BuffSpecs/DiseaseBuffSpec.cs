using UnityEngine;

[CreateAssetMenu(fileName = "DiseaseBuffSpec", menuName = "Smolbean/Buffs/Disease Buff", order = 1)]
public class DiseaseBuffSpec : BuffSpec
{
    public float probabilitySeconds;
    public float minAgeSeconds;
    public float maxAgeSeconds;
    public float minTimeOnIslandSeconds;

    public bool isPermanent;
    public float minDurationSeconds;
    public float maxDurationSeconds;
    public float healthDecreasePerSecond;
    public float foodDecreasePerSecond;
    public float speedMultiplierOneOff = 1.0f;
    public float oldAgeDiseaseChanceMultiplier = 1.0f;
    public float minDistanceTravelledPerDay;
    public float maxDistanceTravelledPerDay;

    public override BuffInstance GetBuff()
    {
        float duration = Random.Range(minDurationSeconds, maxDurationSeconds);
        return new DiseaseBuffInstance(duration) { Spec = this };
    }

    public bool CheckStart(AnimalStats stats, AnimalSpec species, float timeDelta)
    {
        if (stats.age < minAgeSeconds || (stats.age > maxAgeSeconds && maxAgeSeconds != 0.0f))
            return false;

        if (stats.timeOnIsland < minTimeOnIslandSeconds)
            return false;

        float ageInDays = stats.timeOnIsland / DayNightCycleController.Instance.DayLengthSeconds;
        float dist = stats.distanceTravelled / ageInDays;
        if (dist < minDistanceTravelledPerDay || (dist > maxDistanceTravelledPerDay && maxDistanceTravelledPerDay != 0.0f))
            return false;

        float p = 1 / probabilitySeconds * timeDelta;
        if (stats.age > species.oldAgeSeconds)
            p *= oldAgeDiseaseChanceMultiplier;

        return Random.Range(0.0f, 1.0f) < p;
    }
}
