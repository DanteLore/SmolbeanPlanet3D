using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    private readonly string natureLayer;
    private readonly string groundLayer;
    private readonly float shotDistance;
    private ResourceCollectionBuilding building;
    private float radius;
    private float maxRadius;
    private Vector3 center;

    public bool InProgress { get { return radius <= maxRadius && hunter.Prey == null; } }

    public SearchForPreyState(Hunter hunter, GridManager gridManager, AnimalSpec targetSpecies, float shootHeight, string creatureLayer, string natureLayer, string groundLayer, float shotDistance)
    {
        this.hunter = hunter;
        this.gridManager = gridManager;
        this.targetSpecies = targetSpecies;
        this.shootHeight = shootHeight;
        this.creatureLayer = creatureLayer;
        this.natureLayer = natureLayer;
        this.groundLayer = groundLayer;
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
        float rotation = 0f;
        int tryCount = 0;

        // Start where we're currently standing
        shootPosition = pos;

        while(!ShootingPositionValid(shootPosition, targetPosition) && tryCount++ < 100)
        {
            // Rotate around a bit
            var rot = Quaternion.AngleAxis(rotation, Vector3.up);
            shootPosition = pos + (rot * (targetPosition - pos).normalized * shotDistance);

            // Clamp to the ground
            float y = gridManager.GetGridHeightAt(shootPosition.x, shootPosition.z);
            shootPosition = new Vector3(shootPosition.x, y, shootPosition.z);

            // Find nearest spot on the navmesh
            NavMesh.SamplePosition(shootPosition, out var hit, 20f, NavMesh.AllAreas);
            shootPosition = hit.position;

            rotation += Random.Range(2f, 10f); // Rotate randomly around for next try (deliberately after pos calculated!)
        }
        
        return shootPosition;
    }

    private bool ShootingPositionValid(Vector3 shootPosition, Vector3 targetPosition)
    {
        var arrowStartPosition = shootPosition + Vector3.up * shootHeight;

        // Because the y coord will have changed, the shot distance check needs to have some leeway as we may be further 
        // away LOS than in pure x/z terms.  Also impose a minimum
        float dist = Vector3.Distance(targetPosition, arrowStartPosition);
        if(dist > shotDistance * 1.2f || dist < shotDistance * 0.2f)
            return false; // Too far away

        // Check LOS
        Ray ray = new Ray(arrowStartPosition, targetPosition - arrowStartPosition);
        return Physics.SphereCast(ray, 0.25f, shotDistance, LayerMask.GetMask(natureLayer, groundLayer));
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
