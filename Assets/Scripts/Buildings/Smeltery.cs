using System;
using System.Collections;
using UnityEngine;

public class Smeltery : FactoryBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject fireObject;
    public ParticleSystem smokeParticleSystem;
    public GameObject smelterPrefab;

    protected override void Start()
    {
        base.Start();
        fireObject.SetActive(false);
        smokeParticleSystem.Stop();
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
        return dropPoint.transform.position;
    }
    
    public override void StartProcessing()
    {
        base.StartProcessing();

        fireObject.SetActive(true);
        smokeParticleSystem.Play();
    }

    public override DropSpec StopProcessing()
    {
        fireObject.SetActive(false);
        smokeParticleSystem.Stop();

        return base.StopProcessing();
    }
}
