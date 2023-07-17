using System.Collections;
using UnityEngine;

public class Storehouse : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;
    public int portersToSpawn = 5;
    public GameObject spawnPoint;
    public GameObject porterPrefab;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(CreatePorter(spawnDelaySeconds));
    }

    private IEnumerator CreatePorter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        for(int i = 0; i < portersToSpawn; i++)
        {
            Instantiate(porterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
            yield return new WaitForSeconds(delayTime);
        }
    }

    public override Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }

    public override Vector3 GetDropPoint()
    {
        return transform.position;
    }
}
