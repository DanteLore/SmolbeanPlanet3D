using UnityEngine;

public class MapInteractionData
{
    public bool StartBuild { get; private set; }
    public bool Cancelled { get; private set; }
    public bool NewObjectClicked { get; private set; }
    public BuildingSpec SelectedBuildingSpec { get; private set; }
    public bool StartWorkAreaPlacement { get; private set; }

    public Vector3 SelectedPoint { get; set; }
    public Transform SelectedTransform { get; set; }
    public Vector3 HitPoint { get; set; }
    public bool OverMenu { get; set; }
    public bool LeftButtonClicked { get; set; }
    public bool RightButtonClicked { get; set; }

    public string NatureLayerName;
    public string CreatureLayerName;
    public string GroundLayerName;
    public string BuildingLayerName;
    public string DropLayerName;
    public LayerMask AllLayers;
    public int NatureLayer;
    public int CreatureLayer;
    public int GroundLayer;
    public int BuildingLayer;
    public int DropLayer;
    internal string[] BuildCollisionLayers;

    public GameObject SelectedGameObject { get { return SelectedTransform != null ? SelectedTransform.gameObject : null; } }

    public T Selected<T>() where T : MonoBehaviour => SelectedGameObject.GetComponent<T>();

    public void ClearSingleFrameFlags()
    {
        StartBuild = false;
        Cancelled = false;
        NewObjectClicked = false;
        StartWorkAreaPlacement = false;
    }

    public void SetStartBuild(BuildingSpec spec)
    {
        StartBuild = true;
        SelectedBuildingSpec = spec;
    }

    public void SetCancelled()
    {
        Cancelled = true;
    }

    public void SetNewObjectClicked()
    {
        NewObjectClicked = true;
    }

    public void SetStartWorkAreaPlacement()
    {
        StartWorkAreaPlacement = true;
    }

    public void ForceDeselect()
    {
        SelectedTransform = null;
    }
}
