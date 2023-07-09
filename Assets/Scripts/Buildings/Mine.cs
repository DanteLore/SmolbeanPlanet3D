using System.Collections;
using UnityEngine;

public class Mine : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject porterPrefab;
    public GameObject spawnPoint;
    public GameObject dropPoint;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(CreateMiner(spawnDelaySeconds));
    }

    private IEnumerator CreateMiner(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Instantiate(porterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
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
