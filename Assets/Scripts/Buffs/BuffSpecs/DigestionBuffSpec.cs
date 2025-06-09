using UnityEngine;

[CreateAssetMenu(fileName = "DigestionBuffSpec", menuName = "Smolbean/Buffs/Digestion Buff", order = 1)]
public class DigestionBuffSpec : BuffSpec
{
    public float foodDigestedPerSecond;
    public float starvationThreshold;
    public float starvationRatePerSecond;
    public float healthRecoveryPerSecond;
    public float maxFoodLevel;
    public float maxHealth;
    public float sleepingDigestionMultiplier;
    public float sleepingHealthDecreaseMultiplier;

    public override BuffInstance GetBuff()
    {
        return new DigestionBuffInstance(this);
    }
}
