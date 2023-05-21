using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonecuttersHut : SmolbeanBuilding
{
    public GameObject spawnPoint;
    public GameObject stonecutterPrefab;
    public float spawnDelaySeconds = 5f;
    private GameObject stonecutter;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(CreateWoodcutter(spawnDelaySeconds));
    }

    private IEnumerator CreateWoodcutter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        stonecutter = Instantiate(stonecutterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
    }

    public override Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }
}
