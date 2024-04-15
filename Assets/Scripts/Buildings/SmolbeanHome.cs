using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmolbeanHome : MonoBehaviour
{
    public int colonistsToSpawn = 4;
    public int maxCapacity = 4;
    public AnimalSpec colonistSpec;
    public string creatureLayer = "Creatures";
    public float initialSpawnDelay = 2f;
    public float intermediateSpawnDelay = 30f;

    private SmolbeanBuilding building;
    private readonly List<SmolbeanColonist> colonists = new();
    public string BuildingName { get { return building.name; }}

    private void Start()
    {
        building = GetComponent<SmolbeanBuilding>();
        Debug.Assert(building != null, "A home must be a building!");

        StartCoroutine(nameof(CreateColonists));
    }

    private IEnumerator CreateColonists()
    {
        yield return new WaitForSeconds(initialSpawnDelay);

        while(colonists.Count < colonistsToSpawn)
        {
            yield return new WaitUntil(SpawnPointEmpty);

            var colonist = (SmolbeanColonist)AnimalController.Instance.CreateAnimal(colonistSpec, building.dropPoint.transform.position);
            colonist.Home = this;
            colonists.Add(colonist);
            
            yield return new WaitForSeconds(intermediateSpawnDelay);
        }
    }

    private bool SpawnPointEmpty()
    {
        return !Physics.CheckSphere(building.dropPoint.transform.position, 2f, LayerMask.GetMask(creatureLayer));
    }

    public void SwapColonist(SmolbeanColonist originalColonist, SmolbeanColonist newColonist)
    {
        colonists.Remove(originalColonist);
        colonists.Add(newColonist);
    }

    public void AddColonist(SmolbeanColonist colonist)
    {
        Debug.Assert(colonists.Count < maxCapacity - 1, "No room for colonist");

        colonists.Add(colonist);
    }
}
