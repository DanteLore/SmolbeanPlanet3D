using System.Collections;
using UnityEngine;

public class Storehouse : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(CreateCourier(spawnDelaySeconds));
    }

    private IEnumerator CreateCourier(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
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
