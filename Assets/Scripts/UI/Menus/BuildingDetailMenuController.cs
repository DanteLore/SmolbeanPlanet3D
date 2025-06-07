using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingDetailsMenuController : SmolbeanMenu
{
    protected UIDocument document;
    private VisualElement root;
    protected GridManager gridManager;
    protected SoundPlayer soundPlayer;
    protected Transform target;
    private SmolbeanBuilding building;
    private Button deleteButton;
    private Button rotateButton;
    private Button placeWorkingAreaButton;

    protected void OnEnable()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        
        target = MapInteractionManager.Instance.Data.SelectedTransform;
        building = target.GetComponent<SmolbeanBuilding>();

        document.rootVisualElement.Q<Button>("closeButton").clicked += CloseButtonClicked;

        deleteButton = document.rootVisualElement.Q<Button>("deleteButton");
        rotateButton = document.rootVisualElement.Q<Button>("rotateButton");
        placeWorkingAreaButton = document.rootVisualElement.Q<Button>("placeWorkingAreaButton");

        deleteButton.clicked += DeleteButtonClicked;
        rotateButton.clicked += RotateButtonClicked;
        placeWorkingAreaButton.clicked += PlaceWorkingAreaClicked;

        deleteButton.visible = target != null && building.BuildingSpec.deleteAllowed;
        placeWorkingAreaButton.visible = building is ResourceCollectionBuilding;

        InvokeRepeating(nameof(DrawMenu), 1f, 1f);

        DrawMenu();
    }

    protected void OnDisable()
    {
        target = null;
        CancelInvoke(nameof(DrawMenu));
    }

    protected void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MapInteractionManager.Instance.Data.ForceDeselect();
        MenuController.Instance.CloseAll();
    }
    private void RotateButtonClicked()
    {
        if(target) 
            target.Rotate(Vector3.up, 90);
    }

    private void DeleteButtonClicked()
    {
        if (target)
            BuildingController.Instance.DeleteBuilding(target.gameObject);
    }

    private void PlaceWorkingAreaClicked()
    {
        MapInteractionManager.Instance.Data.SetStartWorkAreaPlacement();
    }

    private void DrawMenu()
    {
        root.Q<VisualElement>("thumbnail").style.backgroundImage = building.BuildingSpec.thumbnail;
        root.Q<Label>("nameLabel").text = building.BuildingSpec.buildingName;
        var pos = gridManager.GetGameSquareFromWorldCoords(building.transform.position);
        root.Q<Label>("positionLabel").text = $"{pos.x}λ \u00d7 {pos.y}φ";

        if (!building.IsComplete)
            BuildIngredients();

        if(!building.Inventory.IsEmpty())
            BuildInventory();

        if(building is FactoryBuilding factory)
            BuildRecipe();

        var jobs = JobController.Instance.GetAllJobsForBuilding(building).ToArray();
        if (jobs.Length > 0)
            BuildJobs(jobs);

        var home = building.GetComponent<SmolbeanHome>();
        if(home != null)
            BuildResidents();
    }

    private void BuildResidents()
    {
        /*
        Title(mainScrollView, "𐘦", "Residents");

        var residentContainer = new VisualElement();
        residentContainer.AddToClassList("residentContainer");
        mainScrollView.Add(residentContainer);

        foreach(var colonist in home.Colonists)
        {
            VisualElement residentRow = new();
            residentRow.AddToClassList("residentRow");
            residentContainer.Add(residentRow);

            Button colonistButton = new();
            colonistButton.style.backgroundColor = new Color(0, 0, 0, 0);
            colonistButton.style.backgroundImage = colonist.Species.thumbnail;
            residentRow.Add(colonistButton);

            Label colonistLabel = new();
            colonistLabel.text = colonist.Stats.name;
            residentRow.Add(colonistLabel);
        }
        */
    }

    private static void BuildRecipe()
    {
        /*
        Title(mainScrollView, "𐜫", "Recipes");

        var recipeContainer = new VisualElement();
        recipeContainer.AddToClassList("recipeContainer");
        mainScrollView.Add(recipeContainer);

        foreach (var ingredient in factory.recipe.ingredients)
        {
            Button button = new();
            button.text = ingredient.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = ingredient.item.thumbnail;
            recipeContainer.Add(button);
        }

        Label equalsLabel = new();
        equalsLabel.AddToClassList("recipeContainerLabel");
        recipeContainer.Add(equalsLabel);
        equalsLabel.text = "→";

        Button productButton = new();
        productButton.text = factory.recipe.quantity.ToString();
        productButton.style.backgroundImage = factory.recipe.createdItem.thumbnail;
        recipeContainer.Add(productButton);
        */
    }

    private void BuildInventory()
    {
        var inventoryContainer = root.Q<VisualElement>("inventoryContainer");
        inventoryContainer.Clear();

        foreach (var item in building.Inventory.Totals)
        {
            Button button = new();
            button.text = item.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = item.dropSpec.thumbnail;
            inventoryContainer.Add(button);
        }
    }

    private void BuildJobs(Job[] jobs)
    {
        var jobsListView = root.Q<MultiColumnListView>("jobsListView");
        JobViewBuilder.BuildJobView(jobsListView, jobs);
    }

    private static void BuildIngredients()
    {
        /*
        Title(mainScrollView, "⚒", "Materials", "notoEmoji");

        var ingredientContainer = new VisualElement();
        ingredientContainer.AddToClassList("recipeContainer");
        mainScrollView.Add(ingredientContainer);

        foreach (var ingredient in building.BuildingSpec.ingredients)
        {
            Button button = new();
            button.text = ingredient.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = ingredient.item.thumbnail;
            ingredientContainer.Add(button);
        }
        */
    }
}
