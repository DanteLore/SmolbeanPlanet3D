using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;

public class SearchForResourceState : IState
{
    private const float maxRadius = 496f;
    private readonly ResourceGatherer gatherer;
    private readonly string natureLayer;
    private float radius;

    public bool InProgress { get { return radius <= maxRadius; } }

    public SearchForResourceState(ResourceGatherer gatherer, string natureLayer)
    {
        this.gatherer = gatherer;
        this.natureLayer = natureLayer;
    }

    public void OnEnter()
    {
        radius = 32f;
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
                                .Take(10)
                                .Where(IsOnNavMesh)
                                .ToList()
                                .OrderBy(_ => Guid.NewGuid())
                                .FirstOrDefault();

        if (target)
            gatherer.Target = target;
        else
            radius += 8f;
    }

    private IEnumerable<GameObject> GetTargets()
    {
        var building = gatherer.Job.Building as ResourceCollectionBuilding;
        if (building == null)
            return Enumerable.Empty<GameObject>();
        
        Vector3 pos = building.collectionZoneCenter;
        float radius = building.collectionZoneRadius;

        var candidates = Physics.OverlapSphere(pos, radius, LayerMask.GetMask(natureLayer));

        return candidates
            .Select(c => c.gameObject)
            .Where(go => go.GetComponent(gatherer.TargetType) != null)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos))
            .ToList();
    }

    private bool IsOnNavMesh(GameObject obj)
    {
        return NavMesh.SamplePosition(obj.transform.position, out var _, 2f, NavMesh.AllAreas);
    }
}
