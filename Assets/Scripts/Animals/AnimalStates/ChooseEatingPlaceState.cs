using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ChooseEatingPlaceState : IState
{
    private readonly SmolbeanAnimal animal;

    public ChooseEatingPlaceState(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        animal.Think("Looking for food");

        var pos = animal.transform.position;

        List<Vector3> candidates = new();

        for (int i = 0; i <= 6; i++)
        {
            float radius = animal.Species.sightRange / 2f;
            float x = pos.x + Random.Range(-radius, radius);
            float z = pos.z + Random.Range(-radius, radius);

            if (
                    Physics.Raycast(new Ray(new Vector3(x, 1000, z), Vector3.down), out var rayHit, 2000, LayerMask.GetMask("Ground"))
                    && NavMesh.SamplePosition(rayHit.point, out var navHit, 1.0f, NavMesh.AllAreas)
                    && navHit.position.y > 0.0f //don't go into the sea!
                )
            {
                candidates.Add(navHit.position);
            }
        }

        if (candidates.Count == 0)
            animal.Target = pos; // Give up!
        else
            animal.Target = candidates.OrderByDescending(p => GroundWearManager.Instance.GetAvailableGrass(p)).First();
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
