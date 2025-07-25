using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovingBuffInstance : BuffInstance
{
    private float maxGrassAtLocation = 0f;
    private GroundWearManager gwm;
    private float lastSpeedMultiplier = 1f;

    public override void ApplyTo(AnimalStats stats, AnimalSpec species, SmolbeanAnimal animal, float timeDelta, List<BuffInstance> newBuffs)
    {
        var movingBuffSpec = (MovingBuffSpec)Spec;

        var pos = animal.transform.position;
        MoveSlowlyThroughGrass(animal, pos, movingBuffSpec, stats);

        // Move slower on a slope

    }

    private void MoveSlowlyThroughGrass(SmolbeanAnimal animal, Vector3 pos, MovingBuffSpec spec, AnimalStats stats)
    {
        if (gwm == null)
        {
            // First time around
            gwm = GroundWearManager.Instance;
            maxGrassAtLocation = gwm.GetMaxGrass();
        }

        var available = Mathf.Clamp01(gwm.GetAvailableGrass(pos) / maxGrassAtLocation);
        float speedMultiplier = 1f - available * spec.grassSlowdownMultiplier;

        if(Mathf.Abs(lastSpeedMultiplier - speedMultiplier) > 0.05f)
        {
            stats.speed /= lastSpeedMultiplier; // Remove previous multiplier
            stats.speed *= speedMultiplier; // Apply new multiplier
            lastSpeedMultiplier = speedMultiplier;
        }
    }
}
