using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SearchForResourceState : IState
{
    private float maxRadius;
    private Vector3 center;
    private readonly ResourceGatherer gatherer;
    private readonly string natureLayer;
    private ResourceCollectionBuilding building;
    private float radius;

    public bool InProgress { get { return radius <= maxRadius; } }

    public SearchForResourceState(ResourceGatherer gatherer, string natureLayer)
    {
        this.gatherer = gatherer;
        this.natureLayer = natureLayer;
    }

    public void OnEnter()
    {
        building = (ResourceCollectionBuilding)gatherer.Job.Building;
        radius = Math.Min(building.collectionZoneRadius, 8f);
        maxRadius = building.collectionZoneRadius;
        center = building.collectionZoneCenter;
        gatherer.Target = null;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (!gatherer.Target)
            FindTarget();
    }

    private void FindTarget()
    {
        var target = GetTargets()
                        .Where(IsOnNavMesh)
                        .Take(5)
                        .ToList()
                        .OrderBy(_ => Random.Range(0, 100))
                        .FirstOrDefault();

        if (target)
            gatherer.Target = target;
        else
            radius += 8f;
    }

    private IEnumerable<GameObject> GetTargets()
    {   
        var candidates = Physics.OverlapSphere(center, radius, LayerMask.GetMask(natureLayer));

        return candidates
            .Select(c => c.gameObject)
            .Where(go => go.GetComponent(gatherer.TargetType) != null)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - center))
            .ToList();
    }

    private bool IsOnNavMesh(GameObject obj)
    {
        return NavMesh.SamplePosition(obj.transform.position, out var _, 2f, NavMesh.AllAreas);
    }
}
