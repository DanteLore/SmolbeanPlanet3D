using System.Collections;
using UnityEngine;

public class Storehouse : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject porterPrefab;
    private GameObject porter;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(CreatePorter(spawnDelaySeconds));
    }

    private IEnumerator CreatePorter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        porter = Instantiate(porterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
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
