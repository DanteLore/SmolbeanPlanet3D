using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MapInteractionManager : MonoBehaviour
{
    public static MapInteractionManager Instance;
    private StateMachine stateMachine;

    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    public string natureLayerName = "Nature";
    public string creatureLayerName = "Creatures";
    public string groundLayerName = "Ground";
    public string buildingLayerName = "Buildings";
    public string dropLayerName = "Drops";

    public GameObject selectionCursorPrefab;

    private LayerMask allLayers;
    private int natureLayer;
    private int creatureLayer;
    private int groundLayer;
    private int buildingLayer;
    private int dropLayer;
    private bool newObjectClicked;
    private Transform selectedTransform;
    private Vector3 hitPoint;

    public Transform SelectedTransform { get { return selectedTransform; } }

    private GameObject SelectedGameObject { get { return selectedTransform.gameObject; } }
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
        stateMachine = new StateMachine(shouldLog: true);
        SetupLayerData();

        var idle = new MapIdleState();
        var animalSelected = new AnimalSelectedState(this, selectionCursorPrefab);
        var buildingSelected = new BuildingSelectedState(this, selectionCursorPrefab);

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

        stateMachine.SetStartState(idle);

        Func<bool> MapClicked() => () => LeftButtonClicked && SelectedGameObject.layer == groundLayer;
        Func<bool> NewItemClicked<T>() => () => LeftButtonClicked && newObjectClicked && SelectedGameObject.GetComponent<T>() != null;
        Func<bool> NothingSelected() => () => selectedTransform == null;
    }

    void Update()
    {
        bool overMenu = EventSystem.current.IsPointerOverGameObject();

        if (overMenu || GameStateManager.Instance.IsPaused)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, allLayers))
        {
            hitPoint = hit.point;

            if(LeftButtonClicked)
            {
                newObjectClicked = !ReferenceEquals(selectedTransform, hit.transform);
                selectedTransform = hit.transform;
            }
        }

        stateMachine.Tick();
    }

    public void ForceDeselect()
    {
        selectedTransform = null;
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
