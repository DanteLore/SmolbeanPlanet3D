using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;

public class SearchForResourceState : IState
{
    private ResourceGatherer gatherer;
    private string natureLayer;

    public SearchForResourceState(ResourceGatherer gatherer, string natureLayer)
    {
        this.gatherer = gatherer;
        this.natureLayer = natureLayer;
    }

    public void OnEnter()
    {
        gatherer.Target = GetTargets(gatherer.transform.position)
                                .Take(10)
                                .ToList()
                                .OrderBy(_ => Guid.NewGuid())
                                .FirstOrDefault();
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        
    }

    private IEnumerable<GameObject> GetTargets(Vector3 pos)
    {
        var candidates = Physics.OverlapSphere(pos, 500f, LayerMask.GetMask(natureLayer));

        return candidates
            .Select(c => c.gameObject)
            .Where(go => go.GetComponent(gatherer.TargetType) != null)
            .Where(IsOnNavMesh)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos))
            .ToList();
    }

    private bool IsOnNavMesh(GameObject obj)
    {
        return 
                NavMesh.SamplePosition(obj.transform.position, out var hit, 10f, NavMesh.AllAreas)
            &&  Vector3.SqrMagnitude(obj.transform.position - hit.position) <= 2.5f;
    }
}
