using System.Collections;
using UnityEngine;

public class Shipwreck : SmolbeanBuilding
{
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject builderPrefab;
    public float spawnDelaySeconds = 5f;
    private GameObject builder;

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(CreateBuilder(spawnDelaySeconds));
    }

    private IEnumerator CreateBuilder(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        builder = Instantiate(builderPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
    }

    public override Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }

    public override Vector3 GetDropPoint()
    {
        return dropPoint.transform.position;
    }
}
