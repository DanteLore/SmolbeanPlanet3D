using System.Linq;
using UnityEngine;

public class BuildingSelectedState : BaseMapInteractionState
{
    private readonly GameObject selectionCursorPrefab;
    private readonly GameObject spawnPointMarkerPrefab;
    private readonly GameObject circularAreaMarkerPrefab;
    private GameObject cursor;
    private GameObject spawnPointX;
    private GameObject dropPointX;
    private GameObject workingAreaMarker;

    public BuildingSelectedState(MapInteractionData data, GameObject selectionCursorPrefab, GameObject spawnPointMarkerPrefab, GameObject circularAreaMarkerPrefab) : base(data)
    {
        this.selectionCursorPrefab = selectionCursorPrefab;
        this.spawnPointMarkerPrefab = spawnPointMarkerPrefab;
        this.circularAreaMarkerPrefab = circularAreaMarkerPrefab;
    }

    public override void OnEnter()
    {
        var building = data.Selected<SmolbeanBuilding>();
        cursor = Object.Instantiate(selectionCursorPrefab, data.SelectedTransform);
        var pos = cursor.transform.position;
        float y = building.gameObject.GetRendererBounds().max.y + 1f;
        pos = new Vector3(pos.x, y, pos.z);
        cursor.transform.position = pos;

        var spawnPos = building.spawnPoint.transform.position;
        var dropPos = building.dropPoint.transform.position;
        spawnPointX = Object.Instantiate(spawnPointMarkerPrefab, building.spawnPoint.transform);

        if (Vector3.SqrMagnitude(spawnPos - dropPos) >= 4f)
            dropPointX = Object.Instantiate(spawnPointMarkerPrefab, building.dropPoint.transform);

        if (building is ResourceCollectionBuilding rcb)
        {
            workingAreaMarker = Object.Instantiate(circularAreaMarkerPrefab, building.transform);
            workingAreaMarker.transform.position = rcb.collectionZoneCenter;
            float s = rcb.collectionZoneRadius * 2;
            workingAreaMarker.transform.localScale = new Vector3(s, 1000f, s);
        }

        MenuController.Instance.ShowMenu("BuildingDetailsMenu");
    }

    public override void OnExit()
    {
        Object.Destroy(cursor);
        cursor = null;
        if (spawnPointX)
            Object.Destroy(spawnPointX);
        if (dropPointX)
            Object.Destroy(dropPointX);
        if (workingAreaMarker)
            Object.Destroy(workingAreaMarker);

        MenuController.Instance.Close("BuildingDetailsMenu");
    }
}
