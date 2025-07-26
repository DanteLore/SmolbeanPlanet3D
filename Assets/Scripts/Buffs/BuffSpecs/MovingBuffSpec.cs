
using UnityEngine;

[CreateAssetMenu(fileName = "MovingBuffSpec", menuName = "Smolbean/Buffs/Moving Buff", order = 1)]
public class MovingBuffSpec : BuffSpec
{
    public float grassSlowdownMultiplier = 0.5f;
    public string groundLayerName = "Ground";
    [Range(0f, 1f)]
    public float minimumSlopeForPenaltyPercent = 0.1f;
    public float slopeLookaheadDistance = 1.0f;
    public float slopeSmoothingSpeed = 2.0f;
    public float downhillMultiplier = 2.0f;
    public float uphillMultiplier = 0.25f;

    public override BuffInstance GetBuff()
    {
        return new MovingBuffInstance { Spec = this };
    }
}
