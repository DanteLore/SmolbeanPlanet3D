using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonecuttersHut : MonoBehaviour, ISmolbeanBuilding
{
    public GameObject spawnPoint;
    public GameObject stonecutterPrefab;
    public float spawnDelaySeconds = 5f;
    private GameObject stonecutter;
    public BuildingObjectSaveData SaveData { get; set; }

    void Start()
    {
        StartCoroutine(CreateWoodcutter(spawnDelaySeconds));
    }

    private IEnumerator CreateWoodcutter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        stonecutter = Instantiate(stonecutterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
    }

    public Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }
}
