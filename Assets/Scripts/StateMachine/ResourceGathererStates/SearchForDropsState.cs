using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SearchForDropsState : IState
{
    private ResourceGatherer gatherer;
    private string dropLayer;

    public SearchForDropsState(ResourceGatherer gatherer, string dropLayer)
    {
        this.gatherer = gatherer;
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
        if(!gatherer.TargetDrop)
            gatherer.TargetDrop = GetDropTarget();
    }

    private GameObject GetDropTarget()
    {
        return Physics.OverlapSphere(gatherer.transform.position, 5f, LayerMask.GetMask(dropLayer))
            .Select(c => c.gameObject.GetComponent<ItemStack>())
            .Where(i => i != null && i.dropSpec == gatherer.dropSpec)
            .Select(i => i.gameObject)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - gatherer.transform.position))
            .FirstOrDefault();
    }
}
