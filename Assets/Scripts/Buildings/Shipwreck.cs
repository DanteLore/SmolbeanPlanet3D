using System.Collections;
using UnityEngine;

public class Shipwreck : Storehouse
{
    public GameObject dropPoint;
    public GameObject builderPrefab;
    private GameObject builder;

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(CreateBuilder(spawnDelaySeconds));
    }

    private IEnumerator CreateBuilder(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        // Using the drop point as a secondary spawn point
        builder = Instantiate(builderPrefab, dropPoint.transform.position, Quaternion.identity, transform);
    }
}
