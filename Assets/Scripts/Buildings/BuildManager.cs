using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;
    public GameObject mapCursorPrefab;
    public GameObject selectionCursorPrefab;
    public GameObject spawnPointMarkerPrefab;
    public string groundLayer = "Ground";
    public string buildingLayer = "Buildings";
    public string widgetLayer = "Widgets";
    public string[] collisionLayers = { "Nature", "Buildings" };
    public ParticleSystem buildingPlacedParticleSystem;
    public ParticleSystem buildingDeletedParticleSystem;
    public float allowedHeightDifferential = 0.2f;
    public float buildingMarginSize = 0.75f;
    public float cursorOffsetY = 2f;

    private GridManager gridManager;

    private GameObject mapCursor;
    private Vector2Int currentSquare;
    private Vector3 center;
    private bool okToBuild;
    public bool IsBuilding { get; private set; }
    public bool IsEditing { get; private set; }

    public Transform EditTargetTransform { get; private set; }

    private GameObject cursor;
    private int selectedBuildingIndex;
    private SoundPlayer soundPlayer;
    private GameObject spawnPointX;
    private GameObject dropPointX;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else   
            Instance = this;
    }

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        currentSquare = new Vector2Int(int.MaxValue, int.MaxValue);
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        mapCursor = Instantiate(mapCursorPrefab, Vector3.zero, Quaternion.identity, transform.parent);
        mapCursor.SetActive(false);

        GameStateManager.Instance.GamePauseStateChanged += GamePauseStateChanged;
    }

    public void BeginBuild(BuildingSpec spec)
    {
        if(IsEditing)
            EndEdit();

        selectedBuildingIndex = Array.IndexOf(BuildingController.Instance.buildings, spec);
        IsBuilding = true;
    }

    public void EndBuild()
    {
        IsBuilding = false;
        mapCursor.SetActive(false);
    }

    private void BeginEdit(Transform target)
    {
        IsEditing = true;
        EditTargetTransform = target;

        cursor = Instantiate(selectionCursorPrefab, target);
        var pos = cursor.transform.position;
        float y = target.GetComponentInChildren<Renderer>().bounds.max.y + cursorOffsetY;
        pos = new Vector3(pos.x, y, pos.z);
        cursor.transform.position = pos;

        var building = target.gameObject.GetComponent<SmolbeanBuilding>();
        var spawnPos = building.spawnPoint.transform.position;
        var dropPos = building.dropPoint.transform.position;
        spawnPointX = Instantiate(spawnPointMarkerPrefab, building.spawnPoint.transform);

        if(Vector3.SqrMagnitude(spawnPos - dropPos) >= 4f)
            dropPointX = Instantiate(spawnPointMarkerPrefab, building.dropPoint.transform);

        MenuController.Instance.ShowMenu("BuildingDetailsMenu");
    }

    private void EndEdit()
    {
        IsEditing = false;
        EditTargetTransform = null;
        if(cursor) 
            Destroy(cursor);
        if(spawnPointX)
            Destroy(spawnPointX);
        if(dropPointX) 
            Destroy(dropPointX);
        MenuController.Instance.Close("BuildingDetailsMenu");
    }

    public void ClearSelection()
    {
        EndEdit();
    }

    // TODO:  The following three "Update" methods should be illegal.  This is a shameful implementation of a state machine
    //        and if I weren't so tired of this code, I'd do some kind of massive refactor.
    //        as is stands though, just gonna leave it for Future Dan(tm) to sort out!

    // Future Dan(tm) refactored out the building controller stuff to it's own file, so this mess is just the building placement
    // logic now - which is still a mess.  Good luck, Future Dan(tm)

    void Update()
    {
        // Over UI
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (!GameStateManager.Instance.IsStarted)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            EndEdit();
            EndBuild();
        }

        bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if (isOverUI)
        {
            mapCursor.SetActive(false);
            return;
        }

        if (IsBuilding)
            UpdateBuildMode();
        else 
            UpdateSelectBuildingMode();
    }

    private void GamePauseStateChanged(object sender, bool isPaused)
    {
        if (isPaused)
        {
            EndEdit();
            EndBuild();
        }
    }

    private void UpdateSelectBuildingMode()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool hitSomething = Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask(buildingLayer, widgetLayer));

        if(hitSomething && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer(widgetLayer))
        {
            // If we hit a widget, do nothing here
            return;
        }

        if(IsEditing && !hitSomething) 
        {
            // User clicked off the building
            EndEdit();
        }
        else if(IsEditing && hitInfo.transform != EditTargetTransform)
        {
            EndEdit();
            BeginEdit(hitInfo.transform);
        }
        else if(!IsEditing && hitSomething)
        {
            BeginEdit(hitInfo.transform);
        }
    }

    private void UpdateBuildMode()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask(groundLayer)))
        {
            mapCursor.SetActive(false);
            return;
        }

        Vector2Int newSquare = gridManager.GetGameSquareFromWorldCoords(hitInfo.point);

        bool mouseDown = Mouse.current.leftButton.wasPressedThisFrame;

        if (mouseDown && okToBuild)
        {
            BuildingController.Instance.PlaceBuilding(center, selectedBuildingIndex);

            soundPlayer.Play("Thud");
            Instantiate(buildingPlacedParticleSystem, center, Quaternion.identity);

            EndBuild();
        }
        else if (newSquare != currentSquare)
        {
            currentSquare = newSquare;

            Rect squareBounds = gridManager.GetSquareBounds(currentSquare.x, currentSquare.y);
            var prefab = BuildingController.Instance.buildings[selectedBuildingIndex].prefab;
            Bounds buildingBounds = prefab.GetComponentInChildren<MeshRenderer>().bounds;

            for(int i = 0; i < prefab.transform.childCount; i++)
                buildingBounds.Encapsulate(prefab.transform.GetChild(i).position);
            buildingBounds.Expand(buildingMarginSize);

            float worldX = squareBounds.center.x;
            float worldZ = squareBounds.center.y;
            float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

            Bounds bounds = new(squareBounds.center, buildingBounds.size);

            if (!float.IsNaN(worldY))
            {
                center = new Vector3(worldX, worldY, worldZ);
                okToBuild = CheckFlat(bounds) && CheckEmpty(center);

                Color color = okToBuild ? Color.blue : Color.red;
                mapCursor.transform.position = center;
                mapCursor.GetComponent<Renderer>().material.SetColor("_baseColor", color);
                mapCursor.SetActive(true);
            }
        }
    }

    public void DeleteTargetBuilding()
    {
        Instantiate(buildingDeletedParticleSystem, EditTargetTransform.position, EditTargetTransform.rotation);
        Destroy(EditTargetTransform.gameObject); 
        soundPlayer.Play("Demolish");
        EndEdit();
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

    private bool CheckEmpty(Vector3 center)
    {
        var halfExtents = new Vector3(gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f);
        var objects = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask(collisionLayers));
        return objects.Length == 0;
    }

    private bool CheckFlat(Bounds bounds)
    {
        float rayStartHeight = 10000f;
        int groundMask = LayerMask.GetMask(groundLayer);

        var ray0 = new Ray(new Vector3(bounds.center.x, rayStartHeight, bounds.center.y), Vector3.down);
        if (!Physics.Raycast(ray0, out var hit0, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 0: " + hit0.point);

        float height = hit0.point.y;

        var ray1 = new Ray(new Vector3(bounds.min.x, rayStartHeight, bounds.min.y), Vector3.down);
        if (!Physics.Raycast(ray1, out var hit1, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 1: " + hit1.point);

        if (Mathf.Abs(hit1.point.y - height) > allowedHeightDifferential)
            return false;

        var ray2 = new Ray(new Vector3(bounds.max.x, rayStartHeight, bounds.min.y), Vector3.down);
        if(!Physics.Raycast(ray2, out var hit2, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 2: " + hit2.point);

        if (Mathf.Abs(hit2.point.y - height) > allowedHeightDifferential)
            return false;

        var ray3 = new Ray(new Vector3(bounds.min.x, rayStartHeight, bounds.max.y), Vector3.down);
        if(!Physics.Raycast(ray3, out var hit3, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 3: " + hit3.point);

        if (Mathf.Abs(hit3.point.y - height) > allowedHeightDifferential)
            return false;

        var ray4 = new Ray(new Vector3(bounds.max.x, rayStartHeight, bounds.max.y), Vector3.down);
        if(!Physics.Raycast(ray4, out var hit4, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 4: " + hit4.point);

        if (Mathf.Abs(hit4.point.y - height) > allowedHeightDifferential)
            return false;

        return true;
    }
}
