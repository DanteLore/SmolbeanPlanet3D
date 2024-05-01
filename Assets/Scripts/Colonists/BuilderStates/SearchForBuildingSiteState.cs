using UnityEngine;
using System.Linq;

public class SearchForBuildingSiteState : IState
{
    private readonly Builder builder;
    private readonly string buildingLayer;

    public SearchForBuildingSiteState(Builder builder, string buildingLayer)
    {
        this.builder = builder;
        this.buildingLayer = buildingLayer;
    }

    public void OnEnter()
    {
        var newTarget = GetTarget(builder.transform.position);

        if(builder.TargetBuilding == null && newTarget != null)
            builder.Think($"Next building project: {newTarget.name}");

        builder.TargetBuilding = newTarget;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        
    }

    private BuildingSite GetTarget(Vector3 pos)
    {
        return BuildingController.Instance.GetBuildingsOfType<BuildingSite>()
            .Where(b => b.HasIngredients)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos))
            .FirstOrDefault();
    }
}
