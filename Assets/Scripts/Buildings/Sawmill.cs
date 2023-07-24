using System.Collections;
using UnityEngine;

public class Sawmill : FactoryBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject sawyerPrefab;
    public ParticleSystem sawingParticleSystem;

    protected override void Start()
    {
        base.Start();
        
        sawingParticleSystem.Stop();

        StartCoroutine(CreateSawyer(spawnDelaySeconds));
    }

    private IEnumerator CreateSawyer(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        Instantiate(sawyerPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
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

        sawingParticleSystem.Play();
    }

    public override DropSpec StopProcessing()
    {
        sawingParticleSystem.Stop();

        return base.StopProcessing();
    }
}
