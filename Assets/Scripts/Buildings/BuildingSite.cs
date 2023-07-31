using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSite : SmolbeanBuilding
{
    public GameObject spawnPoint;
    public GameObject dropPoint;
    public GameObject plans;

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
}
