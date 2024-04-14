using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ChooseWorkingAreaState : BaseMapInteractionState
{
    private readonly Transform parent;
    private readonly GameObject circularAreaMarkerPrefab;
    private ResourceCollectionBuilding building;
    private GameObject areaMarker;

    public ChooseWorkingAreaState(MapInteractionData data, Transform parent, GameObject circularAreaMarkerPrefab) : base(data)
    {
        this.parent = parent;
        this.circularAreaMarkerPrefab = circularAreaMarkerPrefab;
    }

    public override void OnEnter()
    {
        building = data.Selected<ResourceCollectionBuilding>();
        areaMarker = Object.Instantiate(circularAreaMarkerPrefab, parent);
        areaMarker.transform.position = building.collectionZoneCenter;
        var s = building.collectionZoneRadius * 2;
        areaMarker.transform.localScale = new Vector3(s, 1000f, s);
    }

    public override void OnExit()
    {
        if(!data.Cancelled)
        {
            building.collectionZoneCenter = data.HitPoint;
        }

        Object.Destroy(areaMarker);
    }

    public override void Tick()
    {
        // Don't call base, because we don't want to deselect anything etc
        data.ClearSingleFrameFlags();
        CaptureMouseState();

        if (!data.OverMenu)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask(data.GroundLayerName)))
            {
                data.HitPoint = hit.point;
                areaMarker.transform.position = hit.point;
            }
        }
    }
}
