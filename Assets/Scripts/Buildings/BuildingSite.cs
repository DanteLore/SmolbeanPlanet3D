using UnityEngine;
using System.Linq;

public class BuildingSite : SmolbeanBuilding
{
    public GameObject plans;
    public ParticleSystem buildParticleSystem;
    public int ingredientDeliveryPriority = 5;

    private float buildingWorkDone;
    private bool buildingInProgress;

    public override bool IsComplete
    {
        get
        {
            return buildingWorkDone >= BuildingSpec.buildTime;
        }
    }

    public bool HasIngredients
    {
        get
        {
            return BuildingSpec.ingredients.All(i => Inventory.Contains(i.item, i.quantity));
        }
    }

    public override BuildingSpec BuildingSpec 
    {
        get
        {
            return base.BuildingSpec;
        }
        set
        {
            base.BuildingSpec = value;

            plans.GetComponent<Renderer>().material.mainTexture = value.thumbnail;
        }
    }
    
    protected override void Start()
    {
        base.Start();

        buildingWorkDone = 0f;
        buildingInProgress = false;
        buildParticleSystem.Stop();

        CreateDeliveryRequests();
    }

    protected override void RegisterJobs()
    {
        // Don't register jobs for an incomplete building.
        // Yes, I know this is a nasty way to supress it...
    }

    private void CreateDeliveryRequests()
    {
        foreach(var ingredient in BuildingSpec.ingredients)
        {
            int toOrder = ingredient.quantity - Inventory.ItemCount(ingredient.item);

            while(toOrder > 0)
            {
                int ammt = Mathf.Min(toOrder, ingredient.item.stackSize);
                DeliveryManager.Instance.CreateDeliveryRequest(this, ingredient.item, ammt, minimum:ammt, priority:ingredientDeliveryPriority);
                toOrder -= ammt;
            }
        }
    }

    public void DoBuild(float amount)
    {
        buildingWorkDone += amount;
    }

    public void StartBuild()
    {
        if(buildingInProgress)
            return;

        buildingInProgress = true;
        buildParticleSystem.Play();
        Inventory.Clear();
    }

    public void EndBuild()
    {
        if(!buildingInProgress)
            return;

        buildingInProgress = false;
        buildParticleSystem.Stop();

        BuildingController.Instance.CompleteBuild(this);
    }
}
