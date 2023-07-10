using System.Collections;
using UnityEngine;

public class Sawmill : SmolbeanBuilding
{
    public float spawnDelaySeconds = 5f;
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject sawyerPrefab;

    public Inventory Inventory { get; private set; }

    protected override void Start()
    {
        base.Start();

        Inventory = new Inventory();

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
        return transform.position;
    }
}
