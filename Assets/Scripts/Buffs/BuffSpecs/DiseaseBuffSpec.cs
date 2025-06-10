using UnityEngine;

[CreateAssetMenu(fileName = "DiseaseBuffSpec", menuName = "Smolbean/Buffs/Disease Buff", order = 1)]
public class DiseaseBuffSpec : BuffSpec
{
    public float probabilitySeconds;
    public bool isPermanent;
    public float minDurationSeconds;
    public float maxDurationSeconds;
    public float healthDecreasePerSecond;
    public float foodDecreasePerSecond;

    public override BuffInstance GetBuff()
    {
        float duration = Random.Range(minDurationSeconds, maxDurationSeconds);
        return new DiseaseBuffInstance(this, duration);
    }
}
