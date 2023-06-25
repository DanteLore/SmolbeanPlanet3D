using System.Collections;
using UnityEngine;

public class Mine : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;

    protected override void Start()
    {
        base.Start();
    }

    public override Vector3 GetSpawnPoint()
    {
        return transform.position;
    }

    public override Vector3 GetDropPoint()
    {
        return transform.position;
    }
}
