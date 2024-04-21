using System;
using UnityEngine;
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

    public GameObject selectionCursorPrefab;
    public GameObject buildingPlacementCursorPrefab;
    public GameObject spawnPointMarkerPrefab;
    public GameObject circularAreaMarkerPrefab;
    public GameObject selectedCircularAreaMarkerPrefab;
    public GameObject buildingPlacedParticleSystem;

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
        stateMachine = new StateMachine(shouldLog: false, allowSelfTransitions: true);
        SetupLayerData();

        var idle = new MapInteractionIdleState(Data);
        var animalSelected = new AnimalSelectedState(Data, selectionCursorPrefab);
        var buildingSelected = new BuildingSelectedState(Data, selectionCursorPrefab, spawnPointMarkerPrefab, circularAreaMarkerPrefab);
        var chooseBuildingLocation = new ChooseBuildingLocationState(Data, transform, buildingPlacementCursorPrefab, gridManager, groundLayerName, Data.BuildCollisionLayers);
        var placeBuilding = new PlaceBuildingState(Data, soundPlayer, buildingPlacedParticleSystem);
        var chooseWorkingArea = new ChooseWorkingAreaState(Data, transform, selectedCircularAreaMarkerPrefab);

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

        AT(buildingSelected, chooseWorkingArea, WorkingAreaButtonClicked());
        AT(chooseWorkingArea, buildingSelected, WorkAreaPlacementFinished());

        stateMachine.SetStartState(idle);

        Func<bool> BuildButtonClicked() => () => Data.StartBuild;
        Func<bool> MapClicked() => () => !Data.OverMenu && Data.LeftButtonClicked && Data.SelectedGameObject.layer == Data.GroundLayer;
        Func<bool> NewItemClicked<T>() where T : MonoBehaviour => () => Data.NewObjectClicked && Data.SelectedGameObject.GetComponent<T>() != null;
        Func<bool> NothingSelected() => () => Data.SelectedTransform == null;
        Func<bool> KeyDown(Key key) => () => Keyboard.current[key].wasPressedThisFrame;
        Func<bool> BuildCancelled() => () => Data.RightButtonClicked || Data.Cancelled;
        Func<bool> BuildTriggered() => () => Data.LeftButtonClicked && !Data.OverMenu && chooseBuildingLocation.okToBuild;
        Func<bool> BuildComplete() => () => placeBuilding.IsComplete;
        Func<bool> WorkingAreaButtonClicked() => () => Data.StartWorkAreaPlacement;
        Func<bool> WorkAreaPlacementFinished() => () => Data.LeftButtonClicked;
    }

    void Update()
    {
        if (!GameStateManager.Instance.IsPaused)
            stateMachine.Tick();
    }

    private void SetupLayerData()
    {
        Data.NatureLayerName = natureLayerName;
        Data.CreatureLayerName = creatureLayerName;
        Data.GroundLayerName = groundLayerName;
        Data.BuildingLayerName = buildingLayerName;
        Data.DropLayerName = dropLayerName;

        Data.AllLayers = LayerMask.GetMask(natureLayerName, creatureLayerName, groundLayerName, buildingLayerName, dropLayerName);
        Data.NatureLayer = LayerMask.NameToLayer(natureLayerName);
        Data.CreatureLayer = LayerMask.NameToLayer(creatureLayerName);
        Data.GroundLayer = LayerMask.NameToLayer(groundLayerName);
        Data.BuildingLayer = LayerMask.NameToLayer(buildingLayerName);
        Data.DropLayer = LayerMask.NameToLayer(dropLayerName);
        Data.BuildCollisionLayers = new string[] { natureLayerName, buildingLayerName };
    }
}
