using UnityEngine;

[CreateAssetMenu(fileName = "AgingBuffSpec", menuName = "Smolbean/Buffs/Aging Buff", order = 1)]
public class AgingBuffSpec : BuffSpec
{
    public float oldAgeSeconds;
    public float lifespanSeconds;
    public float oldAgeHealthImpactPerSecond;
    public float sleepingHealthDecreaseMultiplier;
    public float maturityAgeSeconds;
    [Range(0f, 1f)] public float juvenileScale;

    public DiseaseBuffSpec[] diseases;

    public override BuffInstance GetBuff()
    {
        return new AgingBuffInstance { Spec = this }; 
    }
}
