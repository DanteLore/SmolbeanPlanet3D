using System;
using System.Collections;
using UnityEngine;

public class Smeltery : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject smelterPrefab;
    public DropSpec dropSpec;

    public Inventory Inventory { get; private set; }

    protected override void Start()
    {
        base.Start();

        Inventory = new Inventory();

        StartCoroutine(CreateSmelter(spawnDelaySeconds));
    }

    private IEnumerator CreateSmelter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Instantiate(smelterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
    }

    public override Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }

    public override Vector3 GetDropPoint()
    {
        return transform.position;
    }

    public InventoryItem TryHarvest()
    {
        return null;
        /*
        if(UnityEngine.Random.Range(0f, 1f) < dropProbability)
        {
            TunnelTime += tunnelLengthIncrementPerHarvest;
            return DropController.Instance.CreateInventoryItem(dropSpec, dropSpec.dropRate);
        }
        else
        {
            return null;
        }
        */
    }

    public void StartSmelting()
    {
    }

    public void StopSmelting()
    {
    }
}
