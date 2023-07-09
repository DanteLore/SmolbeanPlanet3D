using System;
using System.Collections;
using UnityEngine;

public class Mine : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject porterPrefab;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public DropSpec dropSpec;
    public float dropProbability = 0.2f;
    public float startingTunnelTime = 2f;
    public float tunnelLengthIncrementPerHarvest = 0.05f;
    public float TunnelTime { get; private set; }

    private Animator animator;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();

        TunnelTime = startingTunnelTime;

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

    public InventoryItem TryHarvest()
    {
        if(UnityEngine.Random.Range(0f, 1f) < dropProbability)
        {
            TunnelTime += tunnelLengthIncrementPerHarvest;
            return DropController.Instance.CreateInventoryItem(dropSpec, dropSpec.dropRate);
        }
        else
        {
            return null;
        }
    }

    public void StartMining()
    {
        animator.SetBool("IsMining", true);
    }

    public void StopMining()
    {
        animator.SetBool("IsMining", false);
    }
}
