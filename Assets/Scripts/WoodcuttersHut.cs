using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodcuttersHut : MonoBehaviour
{
    public GameObject spawnPoint;

    public GameObject woodcutterPrefab;

    public float spawnDelaySeconds = 10f;

    private GameObject woodcutter;

    void Start()
    {
        StartCoroutine(CreateWoodcutter(spawnDelaySeconds));
    }

    private IEnumerator CreateWoodcutter(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        woodcutter = Instantiate(woodcutterPrefab, spawnPoint.transform.position, Quaternion.identity, transform);
    }
}
