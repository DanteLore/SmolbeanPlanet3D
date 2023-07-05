using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class SearchForPorterJobsState : IState
{
    private Porter porter;
    private string dropLayer;

    public SearchForPorterJobsState(Porter porter, string dropLayer)
    {
        this.porter = porter;
        this.dropLayer = dropLayer;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if(!porter.TargetDrop)
            porter.TargetDrop = GetDropTargets(porter.transform.position)
                                    .Take(10)
                                    .ToList()
                                    .OrderBy(_ => Guid.NewGuid())
                                    .FirstOrDefault();
    }

    private IEnumerable<GameObject> GetDropTargets(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, 500f, LayerMask.GetMask(dropLayer))
            .Select(c => c.gameObject.GetComponent<ItemStack>())
            .Where(i => i != null)
            .Select(i => i.gameObject)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - porter.transform.position))
            .ToList();
    }
}
