using System;
using System.Collections.Generic;
using UnityEngine;

public class SearchForPreyState : IState
{
    private const int MAX_COLLIDERS = 10;
    private static readonly Collider[] hitColliders = new Collider[MAX_COLLIDERS];

    private readonly Hunter hunter;
    private readonly AnimalSpec targetSpecies;
    private readonly string creatureLayer;
    private ResourceCollectionBuilding building;
    private float radius;
    private float maxRadius;
    private Vector3 center;

    public bool InProgress { get { return radius <= maxRadius && hunter.Prey == null; } }

    public SearchForPreyState(Hunter hunter, AnimalSpec targetSpecies, string creatureLayer)
    {
        this.hunter = hunter;
        this.targetSpecies = targetSpecies;
        this.creatureLayer = creatureLayer;
    }

    public void OnEnter()
    {
        building = (ResourceCollectionBuilding)hunter.Job.Building;
        radius = Math.Min(building.collectionZoneRadius, 8f);
        maxRadius = building.collectionZoneRadius;
        center = building.collectionZoneCenter;
        hunter.Prey = null;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (hunter.Prey) // Already found 
            return;
            
        var prey = FindTarget();

        if(prey != null)
        {
            hunter.Prey = prey;
            hunter.Think("Selected my target");
        }
        else if(radius < maxRadius)
        {
            radius += 8f;
        }
    }

    private SmolbeanAnimal FindTarget()
    {
        int count = Physics.OverlapSphereNonAlloc(center, radius, hitColliders, LayerMask.GetMask(creatureLayer));

        List<SmolbeanAnimal> animals = new();

        for(int i = 0; i < count; i++)
            if(hitColliders[i].transform.parent.TryGetComponent<SmolbeanAnimal>(out var animal) && animal.Species == targetSpecies) 
                animals.Add(animal);

        if(animals.Count == 0)
            return null;

        animals.Sort((x, y) => {
            return (Vector3.SqrMagnitude(center - x.transform.position) <= Vector3.SqrMagnitude(center - y.transform.position)) ? 1 : -1;
        });

        return animals[0];
    }
}
