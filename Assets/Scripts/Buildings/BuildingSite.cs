using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuildingSite : SmolbeanBuilding
{
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject plans;
    public ParticleSystem buildParticleSystem;

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

    public override Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }

    public override Vector3 GetDropPoint()
    {
        return dropPoint.transform.position;
    }

    protected override void Start()
    {
        base.Start();

        buildingWorkDone = 0f;
        buildingInProgress = false;
        buildParticleSystem.Stop();

        CreateDeliveryRequests();
    }

    private void CreateDeliveryRequests()
    {
        foreach(var ingredient in BuildingSpec.ingredients)
        {
            int toOrder = ingredient.quantity - Inventory.ItemCount(ingredient.item);
            //Debug.Log($"Need {toOrder} of {ingredient.item.dropName}:");

            while(toOrder > 0)
            {
                int ammt = Mathf.Min(toOrder, ingredient.item.stackSize);
                var dr = DeliveryManager.Instance.CreateDeliveryRequest(this, ingredient.item, ammt);
                toOrder -= ammt;
                //Debug.Log(dr);
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

        Debug.Log("Building started");
        buildingInProgress = true;
        buildParticleSystem.Play();
        Inventory.Empty();
    }

    public void EndBuild()
    {
        if(!buildingInProgress)
            return;

        Debug.Log("Building complete");
        buildingInProgress = false;
        buildParticleSystem.Stop();

        BuildManager.Instance.CompleteBuild(this);
    }
}
