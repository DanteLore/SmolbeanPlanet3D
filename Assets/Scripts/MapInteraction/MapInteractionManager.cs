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

    public readonly MapInteractionData Data = new(); 

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
    public GameObject buildingPlacedParticleSystem;

    private LayerMask allLayers;
    private int natureLayer;
    private int creatureLayer;
    private int groundLayer;
    private int buildingLayer;
    private int dropLayer;

    public bool IsOverMap { get { return !Data.OverMenu && Data.SelectedGameObject != null && Data.SelectedGameObject.layer == groundLayer; } }

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
        var animalSelected = new AnimalSelectedState(Data, selectionCursorPrefab);
        var buildingSelected = new BuildingSelectedState(Data, selectionCursorPrefab, spawnPointMarkerPrefab, circularAreaMarkerPrefab);
        var chooseBuildingLocation = new ChooseBuildingLocationState(Data, transform, buildingPlacementCursorPrefab, gridManager, groundLayerName, buildCollisionLayers);
        var placeBuilding = new PlaceBuildingState(Data, soundPlayer, buildingPlacedParticleSystem);

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
        
        Func<bool> BuildButtonClicked() => () => Data.StartBuild;
        Func<bool> MapClicked() => () => !Data.OverMenu && LeftButtonClicked && Data.SelectedGameObject.layer == groundLayer;
        Func<bool> NewItemClicked<T>() where T : MonoBehaviour => () => Data.NewObjectClicked && Data.SelectedGameObject.GetComponent<T>() != null;
        Func<bool> NothingSelected() => () => Data.SelectedTransform == null;
        Func<bool> KeyDown(Key key) => () => Keyboard.current[key].wasPressedThisFrame;
        Func<bool> BuildCancelled() => () => RightButtonClicked || Data.Cancelled;
        Func<bool> BuildTriggered() => () => LeftButtonClicked && chooseBuildingLocation.okToBuild;
        Func<bool> BuildComplete() => () => placeBuilding.IsComplete;
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPaused)
            return;

        Data.OverMenu = EventSystem.current.IsPointerOverGameObject();

        if (!Data.OverMenu)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, allLayers))
            {
                Data.HitPoint = hit.point;

                if (LeftButtonClicked)
                {
                    if (!ReferenceEquals(Data.SelectedTransform, hit.transform))
                        Data.SetNewObjectClicked();
                    Data.SelectedTransform = hit.transform;
                }
            }
        }

        stateMachine.Tick();

        Data.ClearSingleFrameFlags();
    }

    public void ForceDeselect()
    {
        Data.SelectedTransform = null;
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
