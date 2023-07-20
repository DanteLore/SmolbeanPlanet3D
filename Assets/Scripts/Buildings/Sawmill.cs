using System.Collections;
using UnityEngine;

public class Sawmill : FactoryBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject sawyerPrefab;

    protected override void Start()
    {
        base.Start();

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
}
