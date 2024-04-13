using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class WorkingAreaPlacementManager : MonoBehaviour
{
    public static WorkingAreaPlacementManager Instance;
    public GameObject circularAreaMarkerPrefab;
    private string groundLayer = "Ground";
    private ResourceCollectionBuilding building;
    private GameObject areaMarker;
    private bool isEditing = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void StartPlacement(ResourceCollectionBuilding resourceCollectionBuilding)
    {
        building = resourceCollectionBuilding;
        areaMarker = Instantiate(circularAreaMarkerPrefab, transform);
        areaMarker.transform.position = building.collectionZoneCenter;
        var s = building.collectionZoneRadius * 2;
        areaMarker.transform.localScale = new Vector3(s, 1000f, s);
        isEditing = true;
    }

    private void Update()
    {
        if (!isEditing)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hit, float.MaxValue, LayerMask.GetMask(groundLayer)))
            areaMarker.transform.position = hit.point;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if(!EventSystem.current.IsPointerOverGameObject())
                building.collectionZoneCenter = areaMarker.transform.position;
            StartCoroutine(EndEdit());
        }

        if(Mouse.current.rightButton.wasPressedThisFrame)
        {
            StartCoroutine(EndEdit());
        }
    }

    private IEnumerator EndEdit()
    {
        isEditing = false;
        yield return new WaitForEndOfFrame();
        Destroy(areaMarker);
    }
}
