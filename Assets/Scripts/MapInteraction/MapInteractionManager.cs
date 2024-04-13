using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapInteractionManager : MonoBehaviour
{
    public static MapInteractionManager Instance;
    private GridManager gridManager;
    private SoundPlayer soundPlayer;
    private StateMachine stateMachine;

    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    public string natureLayerName = "Nature";
    public string creatureLayerName = "Creatures";
    public string groundLayerName = "Ground";
    public string buildingLayerName = "Buildings";
    public string dropLayerName = "Drops";

    public string[] buildCollisionLayers = new string[] { "Nature", "Buildings" };

    public GameObject selectionCursorPrefab;
    public GameObject buildingPlacementCursorPrefab;
    public GameObject spawnPointMarkerPrefab;
    public GameObject circularAreaMarkerPrefab;

    private LayerMask allLayers;
    private int natureLayer;
    private int creatureLayer;
    private int groundLayer;
    private int buildingLayer;
    private int dropLayer;
    private bool newObjectClicked;
    private Transform selectedTransform;
    private Vector3 hitPoint;
    private bool startBuild = false;
    private BuildingSpec selectedBuildingSpec;
    private bool overMenu;
    private bool cancelled = false;
    private Vector3 selectedPoint;

    public Transform SelectedTransform { get { return selectedTransform; } }
    public Vector3 HitPoint { get { return hitPoint; } }
    public bool IsOverMap { get { return !overMenu && SelectedGameObject != null && SelectedGameObject.layer == groundLayer; } }
    public BuildingSpec SelectedBuildingSpec { get { return selectedBuildingSpec; } }
    public Vector3 SelectedPoint {  get { return selectedPoint; } }

    private GameObject SelectedGameObject { get { return selectedTransform != null ? selectedTransform.gameObject : null; } }
    protected bool LeftButtonClicked { get { return Mouse.current.leftButton.wasPressedThisFrame; } }
    protected bool RightButtonClicked { get { return Mouse.current.rightButton.wasPressedThisFrame; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
        stateMachine = new StateMachine(shouldLog: true, allowSelfTransitions: true);
        SetupLayerData();

        var idle = new MapIdleState();
        var animalSelected = new AnimalSelectedState(this, selectionCursorPrefab);
        var buildingSelected = new BuildingSelectedState(this, selectionCursorPrefab, spawnPointMarkerPrefab, circularAreaMarkerPrefab);
        var chooseBuildingLocation = new ChooseBuildingLocationState(this, buildingPlacementCursorPrefab, gridManager, groundLayerName, buildCollisionLayers);
        var placeBuilding = new PlaceBuildingState(this, soundPlayer);

        AT(idle, KeyDown(Key.Escape));

        AT(idle, animalSelected, NewItemClicked<SmolbeanAnimal>());
        AT(animalSelected, animalSelected, NewItemClicked<SmolbeanAnimal>());
        AT(animalSelected, buildingSelected, NewItemClicked<SmolbeanBuilding>());
        AT(animalSelected, idle, MapClicked());
        AT(animalSelected, idle, NothingSelected());
         
        AT(idle, buildingSelected, NewItemClicked<SmolbeanBuilding>());
        AT(buildingSelected, buildingSelected, NewItemClicked<SmolbeanBuilding>());
        AT(buildingSelected, animalSelected, NewItemClicked<SmolbeanAnimal>());
        AT(buildingSelected, idle, MapClicked());
        AT(buildingSelected, idle, NothingSelected());

        AT(idle, chooseBuildingLocation, BuildButtonClicked());
        AT(chooseBuildingLocation, chooseBuildingLocation, BuildButtonClicked());
        AT(chooseBuildingLocation, idle, BuildCancelled());
        AT(chooseBuildingLocation, placeBuilding, BuildTriggered());
        AT(placeBuilding, idle, BuildComplete());

        stateMachine.SetStartState(idle);
        
        Func<bool> BuildButtonClicked() => () => startBuild;
        Func<bool> MapClicked() => () => !overMenu && LeftButtonClicked && SelectedGameObject.layer == groundLayer;
        Func<bool> NewItemClicked<T>() => () => !overMenu && LeftButtonClicked && newObjectClicked && SelectedGameObject.GetComponent<T>() != null;
        Func<bool> NothingSelected() => () => selectedTransform == null;
        Func<bool> KeyDown(Key key) => () => Keyboard.current[key].wasPressedThisFrame;
        Func<bool> BuildCancelled() => () => RightButtonClicked || cancelled;
        Func<bool> BuildTriggered() => () => LeftButtonClicked && chooseBuildingLocation.okToBuild;
        Func<bool> BuildComplete() => () => placeBuilding.IsComplete;
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPaused)
            return;

        overMenu = EventSystem.current.IsPointerOverGameObject();

        if (!overMenu)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, allLayers))
            {
                hitPoint = hit.point;

                if (LeftButtonClicked)
                {
                    newObjectClicked = !ReferenceEquals(selectedTransform, hit.transform);
                    selectedTransform = hit.transform;
                }
            }
        }

        stateMachine.Tick();

        // Clear single frame flags
        startBuild = false;
        cancelled = false;
    }

    public void ForceDeselect()
    {
        selectedTransform = null;
    }

    public void StartBuild(BuildingSpec spec)
    {
        startBuild = true;
        selectedBuildingSpec = spec;
    }

    public void Cancel()
    {
        cancelled = true;
    }

    public void SetSelectedPoint(Vector3 point)
    {
        selectedPoint = point;
    }

    private void SetupLayerData()
    {
        allLayers = LayerMask.GetMask(natureLayerName, creatureLayerName, groundLayerName, buildingLayerName, dropLayerName);
        natureLayer = LayerMask.NameToLayer(natureLayerName);
        creatureLayer = LayerMask.NameToLayer(creatureLayerName);
        groundLayer = LayerMask.NameToLayer(groundLayerName);
        buildingLayer = LayerMask.NameToLayer(buildingLayerName);
        dropLayer = LayerMask.NameToLayer(dropLayerName);
    }
}
