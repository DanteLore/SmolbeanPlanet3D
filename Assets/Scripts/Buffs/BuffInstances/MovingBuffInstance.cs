using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingBuffInstance : BuffInstance
{
    private const float rayHeight = 10.0f;
    private const float rayLength = 20.0f;

    private float maxGrassAtLocation = 0f;
    private GroundWearManager gwm;
    private float lastSpeedMultiplier = 1f;
    private float lastSlopeMultiplier = 1f;
    private float smoothedSlope;
    private Vector3 lastPos;

    public override void ApplyTo(AnimalStats stats, AnimalSpec species, SmolbeanAnimal animal, float timeDelta, List<BuffInstance> newBuffs)
    {
        var movingBuffSpec = (MovingBuffSpec)Spec;

        var transform = animal.transform;
        transform.GetPositionAndRotation(out var pos, out var rot);

        if (pos == lastPos)
            return;

        lastPos = pos;

        int mask = LayerMask.GetMask(movingBuffSpec.groundLayerName);

        MoveSlowlyThroughGrass(pos, movingBuffSpec, stats);
        AdjustSpeedBasedOnSlope(stats, timeDelta, movingBuffSpec, pos, rot, mask);
    }

    private void AdjustSpeedBasedOnSlope(AnimalStats stats, float timeDelta, MovingBuffSpec movingBuffSpec, Vector3 pos, Quaternion rot, int mask)
    {
        float slope = GetSlopeCoefficient(pos, rot, mask, movingBuffSpec.slopeLookaheadDistance);

        smoothedSlope += (slope - smoothedSlope) * movingBuffSpec.slopeSmoothingSpeed * timeDelta;

        if (smoothedSlope < movingBuffSpec.minimumSlopeForPenaltyPercent)
            return; // Not enough slope to make a difference

        float slopeMultiplier;
        if (smoothedSlope >= 0f)
            slopeMultiplier = Mathf.Lerp(1f, movingBuffSpec.downhillMultiplier, smoothedSlope);
        else
            slopeMultiplier = Mathf.Lerp(1f, movingBuffSpec.uphillMultiplier, -smoothedSlope);

        if (Mathf.Abs(lastSlopeMultiplier - slopeMultiplier) > 0.05f)
        {
            stats.speed /= lastSlopeMultiplier; // Remove previous multiplier
            stats.speed *= slopeMultiplier; // Apply new multiplier
            lastSlopeMultiplier = slopeMultiplier;
        }
    }

    private static float GetSlopeCoefficient(Vector3 position, Quaternion orientation, LayerMask groundMask, float lookaheadDistance)
    {
        // helper to raycast down from a world point:
        bool RayDown(Vector3 origin, out RaycastHit hit) =>
            Physics.Raycast(origin + Vector3.up * rayHeight, Vector3.down, out hit, rayLength, groundMask);

        Vector3 forwardDir = (orientation * Vector3.forward).normalized;
        Vector3 p1 = position;
        Vector3 p2 = position + forwardDir * lookaheadDistance ;

        if (!RayDown(p1, out var hit1) || !RayDown(p2, out var hit2))
            return 0f;  // couldn’t get a valid slope

        float deltaY  = hit1.point.y - hit2.point.y;
        float gradient = deltaY / lookaheadDistance;  

        // -1 = 45 degree upward slope 0 = flat, 1 = 45 degree downward slope
        return Mathf.Clamp(gradient, -1f, 1f);
    }

    private void MoveSlowlyThroughGrass(Vector3 pos, MovingBuffSpec spec, AnimalStats stats)
    {
        if (gwm == null)
        {
            // First time around
            gwm = GroundWearManager.Instance;
            maxGrassAtLocation = gwm.GetMaxGrass();
        }

        var available = Mathf.Clamp01(gwm.GetAvailableGrass(pos) / maxGrassAtLocation);
        float speedMultiplier = 1f - available * spec.grassSlowdownMultiplier;

        if (Mathf.Abs(lastSpeedMultiplier - speedMultiplier) > 0.05f)
        {
            stats.speed /= lastSpeedMultiplier; // Remove previous multiplier
            stats.speed *= speedMultiplier; // Apply new multiplier
            lastSpeedMultiplier = speedMultiplier;
        }
    }
}
