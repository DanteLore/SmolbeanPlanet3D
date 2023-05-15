using UnityEngine;

public interface ISmolbeanBuilding
{
    public BuildingObjectSaveData SaveData {get; set;}

    public Vector3 GetSpawnPoint();
}
