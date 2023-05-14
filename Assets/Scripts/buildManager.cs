using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildManager : MonoBehaviour, IObjectGenerator
{
    public static BuildManager Instance;
    public GameObject mapCursorPrefab;
    public GameObject buildingPrefab;
    public string groundLayer = "Ground";
    public string[] collisionLayers = { "Nature", "Buildings", "Creatures" };
    private GridManager gridManager;
    private GameMapGenerator gameMapGenerator;
    private GameObject mapCursor;
    private Vector2Int currentSquare;
    private Vector3 center;
    private bool okToBuild;
    public bool IsBuilding { get; private set; }

    public void BeginBuild()
    {
        IsBuilding = true;
    }

    public void EndBuild()
    {
        IsBuilding = false;
        mapCursor.SetActive(false);
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else   
            Instance = this;
    }

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        gameMapGenerator = FindObjectOfType<GameMapGenerator>();
        currentSquare = new Vector2Int(int.MaxValue, int.MaxValue);

        mapCursor = Instantiate(mapCursorPrefab, Vector3.zero, Quaternion.identity, transform.parent);
        mapCursor.SetActive(false);
    }

    void Update()
    {
        if(GameStateManager.Instance.IsPaused || !IsBuilding)
            return;

        bool isOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        if(isOverUI)
        {
            mapCursor.SetActive(false);
            return;
        }

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(!Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask(groundLayer)))
        {
            mapCursor.SetActive(false);
            return;
        }

        Vector2Int newSquare = gridManager.GetGameSquareFromWorldCoords(hitInfo.point);

        bool mouseDown = Input.GetMouseButtonDown(0);

        if(mouseDown && okToBuild)
        {
            PlaceBuilding(center);
            EndBuild();
        }
        else if(newSquare != currentSquare)
        {
            currentSquare = newSquare;

            int level = gridManager.GameMap[currentSquare.y * gameMapGenerator.mapWidth + currentSquare.x];

            var bounds = gridManager.GetSquareBounds(currentSquare.x, currentSquare.y);

            float worldX = bounds.center.x;
            float worldZ = bounds.center.y;
            float worldY = gridManager.GetGridHeightAt(worldX, worldZ);
            center = new Vector3(worldX, worldY, worldZ);
            okToBuild = CheckFlat(bounds) && CheckEmpty(center);

            Color color = okToBuild ? Color.blue : Color.red;
            mapCursor.transform.position = center;
            mapCursor.GetComponent<Renderer>().material.SetColor("_baseColor", color);
            mapCursor.SetActive(true);
        }
    }

    private void PlaceBuilding(Vector3 pos)
    {
        BuildingObjectSaveData saveData = new BuildingObjectSaveData
        {
            positionX = pos.x,
            positionY = pos.y,
            positionZ = pos.z,
            rotationY = 0,
            prefabIndex = 0
        };

        InstantiateBuilding(saveData);
    }

    private void InstantiateBuilding(BuildingObjectSaveData saveData)
    {
        Vector3 pos = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);

        var building = Instantiate(buildingPrefab, pos, Quaternion.identity, transform);
        building.GetComponent<ISmolbeanBuilding>().SaveData = saveData;
    }

    private bool CheckEmpty(Vector3 center)
    {
        Vector3 boxCentre = new Vector3(center.x, center.y + gridManager.tileSize / 2.0f, center.z);
        Vector3 halfExtents = new Vector3(gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f);
        var objects = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask(collisionLayers));
        return objects.Length == 0;
    }

    private bool CheckFlat(Rect bounds)
    {
        float marginSize = 0.05f;
        float allowedHeightDifferential = 0.2f;
        float rayStartHeight = 1000f;

        float rayLength = 2.0f * rayStartHeight;
        float margin = bounds.width * marginSize;

        var ray1 = new Ray(new Vector3(bounds.xMin + margin, rayStartHeight, bounds.yMin + margin), Vector3.down);
        if(!Physics.Raycast(ray1, out var hit1, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        float height = hit1.point.y;

        var ray2 = new Ray(new Vector3(bounds.xMax - margin, rayStartHeight, bounds.yMin + margin), Vector3.down);
        if(!Physics.Raycast(ray2, out var hit2, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        if(Mathf.Abs(hit2.point.y - height) > allowedHeightDifferential)
            return false;

        var ray3 = new Ray(new Vector3(bounds.xMin + margin, rayStartHeight, bounds.yMax - margin), Vector3.down);
        if(!Physics.Raycast(ray3, out var hit3, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        if(Mathf.Abs(hit3.point.y - height) > allowedHeightDifferential)
            return false;

        var ray4 = new Ray(new Vector3(bounds.xMax - margin, rayStartHeight, bounds.yMax - margin), Vector3.down);
        if(!Physics.Raycast(ray4, out var hit4, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        if(Mathf.Abs(hit4.point.y - height) > allowedHeightDifferential)
            return false;

        return true;
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public List<BuildingObjectSaveData> GetSaveData()
    {
        return GetComponentsInChildren<ISmolbeanBuilding>().Select(b => b.SaveData).ToList();
    }

    public void LoadBuildings(List<BuildingObjectSaveData> loadedData)
    {
        Clear();

        foreach(var buildingData in loadedData)
            InstantiateBuilding(buildingData);
    }
}
