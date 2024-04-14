using UnityEngine;

public class MapInteractionData
{
    public bool StartBuild { get; private set; }
    public bool Cancelled { get; private set; }
    public bool NewObjectClicked { get; private set; }
    public BuildingSpec SelectedBuildingSpec { get; private set; }

    public Vector3 SelectedPoint { get; set; }
    public Transform SelectedTransform { get; set; }
    public Vector3 HitPoint { get; set; }
    public bool OverMenu { get; set; }

    public GameObject SelectedGameObject { get { return SelectedTransform != null ? SelectedTransform.gameObject : null; } }

    public void ClearSingleFrameFlags()
    {
        StartBuild = false;
        Cancelled = false;
        NewObjectClicked = false;
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
}
