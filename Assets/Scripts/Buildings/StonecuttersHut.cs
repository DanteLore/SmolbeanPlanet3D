using System.Collections;
using UnityEngine;

public class StonecuttersHut : SmolbeanBuilding
{
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
}
