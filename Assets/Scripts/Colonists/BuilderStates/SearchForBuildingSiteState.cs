using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.AI;

public class SearchForBuildingSiteState : IState
{
    private Builder builder;
    private string buildingLayer;

    public SearchForBuildingSiteState(Builder builder, string buildingLayer)
    {
        this.builder = builder;
        this.buildingLayer = buildingLayer;
    }

    public void OnEnter()
    {
        builder.TargetBuilding = GetTarget(builder.transform.position);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        
    }

    private BuildingSite GetTarget(Vector3 pos)
    {
        var candidates = Physics.OverlapSphere(pos, 500f, LayerMask.GetMask(buildingLayer));

        return candidates
            .Select(c => c.gameObject.GetComponent<BuildingSite>())
            .Where(b => b != null)
            .Where(b => b.HasIngredients)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos))
            .FirstOrDefault();
    }
}
