using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SearchForPreyState : IState
{
    private const int MAX_COLLIDERS = 100;
    private static readonly Collider[] hitColliders = new Collider[MAX_COLLIDERS];

    private readonly Hunter hunter;
    private readonly GridManager gridManager;
    private readonly AnimalSpec targetSpecies;
    private readonly float shootHeight;
    private readonly string creatureLayer;
    private readonly float shotDistance;
    private ResourceCollectionBuilding building;
    private float radius;
    private float maxRadius;
    private Vector3 center;

    public bool InProgress { get { return radius <= maxRadius && hunter.Prey == null; } }

    public SearchForPreyState(Hunter hunter, GridManager gridManager, AnimalSpec targetSpecies, float shootHeight, string creatureLayer, float shotDistance)
    {
        this.hunter = hunter;
        this.gridManager = gridManager;
        this.targetSpecies = targetSpecies;
        this.shootHeight = shootHeight;
        this.creatureLayer = creatureLayer;
        this.shotDistance = shotDistance;
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
            hunter.Target = SelectShootPosition(prey.transform.position);
            hunter.Think("Selected my target, stalking it now");
        }
        else if(radius < maxRadius)
        {
            radius += 8f;
        }
    }

    private Vector3 SelectShootPosition(Vector3 targetPosition)
    {
        Vector3 shootPosition;
        Vector3 pos = hunter.transform.position;

        do
        {
            shootPosition = pos + ((targetPosition - pos).normalized * shotDistance);

            float y = gridManager.GetGridHeightAt(shootPosition.x, shootPosition.z) + shootHeight;

            shootPosition = new Vector3(shootPosition.x, y, shootPosition.z);

        } while(!ShootingPositionValid(shootPosition, targetPosition)); 

        return shootPosition;
    }

    private bool ShootingPositionValid(Vector3 shootPosition, Vector3 targetPosition)
    {
        return true;
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
