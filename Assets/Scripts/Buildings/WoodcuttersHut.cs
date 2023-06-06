using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcuttersHut : SmolbeanBuilding
{
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject woodcutterPrefab;
    public float spawnDelaySeconds = 5f;
    private GameObject woodcutter;

    protected override void Start()
    {
        base.Start();
        
        StartCoroutine(CreateWoodcutter(spawnDelaySeconds));
    }

    private IEnumerator CreateWoodcutter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        woodcutter = Instantiate(woodcutterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
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
