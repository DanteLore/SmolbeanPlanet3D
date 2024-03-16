using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PorterSearchForDropToCollectState : IState
{
    private const float maxRadius = 496f;

    public bool InProgress { get { return radius <= maxRadius; } }
    private Porter porter;
    private string dropLayer;
    private float radius;

    public PorterSearchForDropToCollectState(Porter porter, string dropLayer)
    {
        this.porter = porter;
        this.dropLayer = dropLayer;
    }

    public void OnEnter()
    {
        porter.Think("Looking for drops to collect");
        radius = 32f;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (!porter.TargetDrop)
            ClaimCollectionJob();
    }

    private void ClaimCollectionJob()
    {
        porter.TargetDrop = GetDropTargets(porter.transform.position)
                                            .Take(3)
                                            .ToList()
                                            .OrderBy(_ => Guid.NewGuid())
                                            .FirstOrDefault();
        if (porter.TargetDrop)
            DeliveryManager.Instance.ClaimCollection(porter.TargetDrop.GetComponent<SmolbeanDrop>(), porter);
        else
            radius += 8f;
    }

    private IEnumerable<GameObject> GetDropTargets(Vector3 pos)
    {
        return Physics.OverlapSphere(pos, radius, LayerMask.GetMask(dropLayer))
            .Select(c => c.gameObject.GetComponent<SmolbeanDrop>())
            .Where(i => i != null)
            .Where(i => !DeliveryManager.Instance.IsCollectionClaimed(i))
            .Select(i => i.gameObject)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - porter.transform.position));
    }
}
