using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PorterSearchForDropToCollectState : IState
{
    private const float MAX_RADIUS = 3200f;
    
    private const int MAX_COLLIDERS = 100;
    private static readonly Collider[] hitColliders = new Collider[MAX_COLLIDERS];

    public bool InProgress { get { return radius <= MAX_RADIUS; } }
    private Porter porter;
    private readonly string dropLayer;
    private float radius;

    public PorterSearchForDropToCollectState(Porter porter, string dropLayer)
    {
        this.porter = porter;
        this.dropLayer = dropLayer;
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
        if (!porter.TargetDrop)
            ClaimCollectionJob();
    }

    private void ClaimCollectionJob()
    {
        porter.TargetDrop = GetDropTarget(porter.transform.position);
                                          
        if (porter.TargetDrop)
            DeliveryManager.Instance.ClaimCollection(porter.TargetDrop.GetComponent<SmolbeanDrop>(), porter);
        else
            radius += 8f;
    }

    private GameObject GetDropTarget(Vector3 pos)
    {
        int count = Physics.OverlapSphereNonAlloc(pos, radius, hitColliders, LayerMask.GetMask(dropLayer));

        List<GameObject> dropObjects = new();

        for(int i = 0; i < count; i++)
            if(hitColliders[i].TryGetComponent<SmolbeanDrop>(out var drop) && !DeliveryManager.Instance.IsCollectionClaimed(drop))
                dropObjects.Add(drop.gameObject);

        if(dropObjects.Count == 0)
            return null;

        dropObjects.Sort((x, y) => {
            return (Vector3.SqrMagnitude(pos - x.transform.position) <= Vector3.SqrMagnitude(pos - y.transform.position)) ? 1 : -1;
        });

        return dropObjects[0];
    }
}
