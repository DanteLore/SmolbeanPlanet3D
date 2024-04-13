using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public GameObject mapCursorPrefab;
    public GameObject selectionCursorPrefab;
    public GameObject spawnPointMarkerPrefab;
    public GameObject circularAreaMarkerPrefab;
    public string groundLayer = "Ground";
    public string buildingLayer = "Buildings";
    public string widgetLayer = "Widgets";
    public string[] collisionLayers = { "Nature", "Buildings" };
    public ParticleSystem buildingPlacedParticleSystem;
    public float allowedHeightDifferential = 0.2f;
    public float buildingMarginSize = 0.75f;
    public float cursorOffsetY = 2f;

    public bool IsBuilding { get; private set; }
    public bool IsEditing { get; private set; }

    public SmolbeanBuilding EditTarget { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else   
            Instance = this;
    }

    private static SmolbeanBuilding GetBuilding(Transform target)
    {
        var building = target.gameObject.GetComponent<SmolbeanBuilding>();

        if (building != null)
            return building;

        if (target.gameObject.GetComponent<BuildManager>() != null)
            return null;

        return GetBuilding(target.parent);
    }

    public SmolbeanBuilding CompleteBuild(BuildingSite site)
    {
        BuildingObjectSaveData saveData = new()
        {
            positionX = site.transform.position.x,
            positionY = site.transform.position.y,
            positionZ = site.transform.position.z,
            rotationY = site.transform.rotation.eulerAngles.y,
            prefabIndex = site.PrefabIndex,
            complete = true
        };

        DestroyImmediate(site.gameObject);
        return BuildingController.Instance.InstantiateBuilding(saveData);
    }
}
