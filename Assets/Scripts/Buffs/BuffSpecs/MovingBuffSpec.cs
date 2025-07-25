
using UnityEngine;

[CreateAssetMenu(fileName = "MovingBuffSpec", menuName = "Smolbean/Buffs/Moving Buff", order = 1)]
public class MovingBuffSpec : BuffSpec
{
    internal float grassSlowdownMultiplier = 0.5f;

    public override BuffInstance GetBuff()
    {
        return new MovingBuffInstance { Spec = this };
    }
}
