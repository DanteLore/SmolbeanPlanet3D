using UnityEngine;
using System.Linq;

public class SearchForDropsState : IState
{
    private readonly ResourceGatherer gatherer;
    private readonly string dropLayer;

    public SearchForDropsState(ResourceGatherer gatherer, string dropLayer)
    {
        this.gatherer = gatherer;
        this.dropLayer = dropLayer;
    }

    public void OnEnter()
    {
        gatherer.TargetDrop = null;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (!gatherer.TargetDrop)
        {
            var target = GetDropTarget();

            if (target != null)
            {
                gatherer.TargetDrop = target.gameObject;
                gatherer.Think($"Picking up {target.quantity} {target.dropSpec.dropName}");
            }
        }
    }

    private SmolbeanDrop GetDropTarget()
    {
        return Physics.OverlapSphere(gatherer.transform.position, 5f, LayerMask.GetMask(dropLayer))
            .Select(c => c.gameObject.GetComponent<SmolbeanDrop>())
            .Where(i => i != null && i.dropSpec == gatherer.DropSpec)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - gatherer.transform.position))
            .FirstOrDefault();
    }
}
