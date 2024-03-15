using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ChooseFlockLocation
    : IState
{
    private readonly SmolbeanAnimal animal;

    public ChooseFlockLocation(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        animal.Think("Following the flock");

        var pos = animal.transform.position;
        var treeLocations = GetTreeLocations(pos);
        var animalLocations = GetAnimalLocations(pos);

        var allLocations = treeLocations.Concat(animalLocations).ToArray();

        float x = 0f, z = 0f;

        if (allLocations.Length > 0)
        {
            foreach (var v in treeLocations)
            {
                x += v.x;
                z += v.z;
            }

            x = (x / allLocations.Length) + Random.Range(-8, 8);
            z = (z / allLocations.Length) + Random.Range(-8, 8);
        }
        else
        {
            x = pos.x + Random.Range(-16, 16);
            z = pos.z + Random.Range(-16, 16);
        }

        if (
                Physics.Raycast(new Ray(new Vector3(x, 1000, z), Vector3.down), out var rayHit, 2000, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(rayHit.point, out var navHit, 2.0f, NavMesh.AllAreas)
                && navHit.position.y > 0.0f //don't go into the sea!
            )
            animal.target = navHit.position;
        else
            animal.target = pos;
    }

    private IEnumerable<Vector3> GetTreeLocations(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, animal.species.sightRange, LayerMask.GetMask("Nature"))
                    .Select(c => c.gameObject.GetComponent<SmolbeanTree>())
                    .Where(_ => _ != null)
                    .Select(tree => tree.transform.position);
    }

    private IEnumerable<Vector3> GetAnimalLocations(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, animal.species.sightRange, LayerMask.GetMask("Creatures"))
                    .Select(c => c.gameObject.GetComponent<SmolbeanAnimal>())
                    .Where(_ => _ != null)
                    .Where(a => a.speciesIndex == animal.speciesIndex)
                    .Select(a => a.transform.position);
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
