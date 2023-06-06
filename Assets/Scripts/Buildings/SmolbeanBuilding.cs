using System;
using UnityEngine;

public enum BuildingWearPattern { Circle, Rectangle }

public abstract class SmolbeanBuilding : MonoBehaviour
{
    public int PrefabIndex {get; set;}
    public BuildingWearPattern wearPattern = BuildingWearPattern.Rectangle;
    public Vector2 wearScale = Vector2.one;
    public GameObject building;

    public abstract Vector3 GetSpawnPoint();
    public abstract Vector3 GetDropPoint();

    protected virtual void Start()
    {
        InvokeRepeating("RegisterWear", 0.0f, 0.5f);
    }

    protected virtual void OnDestroy()
    {
        CancelInvoke("RegisterWear");
    }

    private void RegisterWear()
    {
        GroundWearManager.Instance.BuildingOn(building.GetComponent<Renderer>().bounds, wearPattern, wearScale);
    }

}
