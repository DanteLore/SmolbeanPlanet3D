using System.Linq;
using UnityEngine;

public class ResourceCollectionBuilding : SmolbeanBuilding
{
    public Vector3 collectionZoneCenter;
    public float collectionZoneRadius;

    protected override void Awake()
    {
        base.Awake();

        collectionZoneCenter = transform.position;
    }

    public override BuildingObjectSaveData GetSaveData()
    {
        return new ResourceCollectionBuildingSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            rotationY = transform.rotation.eulerAngles.y,
            prefabIndex = PrefabIndex,
            inventory = Inventory.GetSaveData().ToArray(),
            complete = true,
            collectionZoneCenterX = collectionZoneCenter.x,
            collectionZoneCenterY = collectionZoneCenter.y,
            collectionZoneCenterZ = collectionZoneCenter.z,
        };
    }

    public override void LoadFrom(BuildingObjectSaveData saveData)
    {
        base.LoadFrom(saveData);

        if (saveData is ResourceCollectionBuildingSaveData sd)
            collectionZoneCenter = new Vector3(sd.collectionZoneCenterX, sd.collectionZoneCenterY, sd.collectionZoneCenterZ);
    }
}
