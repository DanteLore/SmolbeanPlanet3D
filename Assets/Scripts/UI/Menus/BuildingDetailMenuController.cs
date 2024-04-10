using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingDetailsMenuController : BaseDetailsMenuController
{
    private BuildManager buildManager;
    private Button deleteButton;
    private Button rotateButton;
    private Button placeWorkingAreaButton;

    protected override void OnEnable()
    {
        base.OnEnable();

        buildManager = FindFirstObjectByType<BuildManager>();
        deleteButton = document.rootVisualElement.Q<Button>("deleteButton");
        rotateButton = document.rootVisualElement.Q<Button>("rotateButton");
        placeWorkingAreaButton = document.rootVisualElement.Q<Button>("placeWorkingAreaButton");

        deleteButton.clicked += DeleteButtonClicked;
        rotateButton.clicked += RotateButtonClicked;
        placeWorkingAreaButton.clicked += PlaceWorkingAreaClicked;
    }

    protected override void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        buildManager.ClearSelection();
        MenuController.Instance.CloseAll();
    }

    protected override void Update()
    {
        if (ReferenceEquals(buildManager.EditTarget, target))
            return;

        target = buildManager.EditTarget.transform;

        Clear();
        UpdateControls();

        if (target != null)
            DrawMenu();
    }

    private void UpdateControls()
    {
        var sbb = target.GetComponent<SmolbeanBuilding>();
        deleteButton.visible = target != null && sbb.BuildingSpec.deleteAllowed;
        placeWorkingAreaButton.visible = sbb is ResourceCollectionBuilding;
    }

    private void RotateButtonClicked()
    {
        if(target) 
            target.Rotate(Vector3.up, 90);
    }

    private void DeleteButtonClicked()
    {
        if (target)
            BuildManager.Instance.DeleteTargetBuilding();
    }

    private void PlaceWorkingAreaClicked()
    {
        WorkingAreaPlacementManager.Instance.StartPlacement(target.GetComponent<ResourceCollectionBuilding>());
    }

    private void DrawMenu()
    {
        var building = target.GetComponent<SmolbeanBuilding>();

        var buildingImage = document.rootVisualElement.Q<VisualElement>("buildingImage");
        buildingImage.style.backgroundImage = building.BuildingSpec.thumbnail;

        var nameLabel = document.rootVisualElement.Q<Label>("nameLabel");
        nameLabel.text = building.BuildingSpec.buildingName;

        var pos = gridManager.GetGameSquareFromWorldCoords(building.transform.position);
        var positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
        positionLabel.text = $"{pos.x}λ \u00d7 {pos.y}φ";

        var mainScrollView = document.rootVisualElement.Q<ScrollView>("mainScrollView");

        if (!building.IsComplete)
            BuildIngredients(building, mainScrollView);

        if(!building.Inventory.IsEmpty())
            BuildInventory(building, mainScrollView);

        if(building is FactoryBuilding factory)
            BuildRecipe(mainScrollView, factory);

        var jobs = JobController.Instance.GetAllJobsForBuilding(building).ToArray();
        if (jobs.Length > 0)
            BuildJobs(mainScrollView, jobs);
    }

    private static void BuildRecipe(ScrollView mainScrollView, FactoryBuilding factory)
    {
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
    }

    private static void BuildInventory(SmolbeanBuilding building, ScrollView mainScrollView)
    {
        Title(mainScrollView, "𐚱", "Inventory");

        var inventoryContainer = new VisualElement();
        inventoryContainer.AddToClassList("inventoryContainer");
        mainScrollView.Add(inventoryContainer);

        foreach (var item in building.Inventory.Totals)
        {
            Button button = new();
            button.text = item.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = item.dropSpec.thumbnail;
            inventoryContainer.Add(button);
        }
    }

    private void BuildJobs(ScrollView mainScrollView, Job[] jobs)
    {
        Title(mainScrollView, "𐛌", "Jobs");

        var jobContainer = new VisualElement();
        jobContainer.AddToClassList("jobContainer");
        mainScrollView.Add(jobContainer);

        foreach (var job in jobs)
        {
            VisualElement jobRow = new();
            jobRow.AddToClassList("jobRow");
            jobContainer.Add(jobRow);

            Toggle jobEnabledToggle = new();
            jobRow.Add(jobEnabledToggle);
            jobEnabledToggle.value = job.IsOpen;
            jobEnabledToggle.RegisterValueChangedCallback(v =>
            {
                if (v.newValue)
                    job.Open();
                else
                    job.Terminate();
            });

            Button jobButton = new();
            jobButton.style.backgroundColor = new Color(0, 0, 0, 0);
            jobButton.style.backgroundImage = job.JobSpec.thumbnail;
            jobRow.Add(jobButton);

            Label jobLabel = new();
            jobLabel.text = job.JobSpec.jobName;
            jobRow.Add(jobLabel);

            if (job.Colonist)
            {
                Button colonistButton = new();
                colonistButton.style.backgroundColor = new Color(0, 0, 0, 0);
                colonistButton.style.backgroundImage = job.Colonist.species.thumbnail;
                jobRow.Add(colonistButton);

                Label colonistLabel = new();
                colonistLabel.text = job.Colonist.Stats.name;
                jobRow.Add(colonistLabel);
            }
        }
    }

    private static void BuildIngredients(SmolbeanBuilding building, ScrollView mainScrollView)
    {
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
    }

    private static void Title(ScrollView mainScrollView, string symbol, string text, string symbolClass = "notoLinearA")
    {
        var titleContainer = new VisualElement();
        titleContainer.AddToClassList("titleRow");
        mainScrollView.Add(titleContainer);

        Label symbolLabel = new();
        symbolLabel.AddToClassList(symbolClass);
        symbolLabel.AddToClassList("bigLabel");
        titleContainer.Add(symbolLabel);
        symbolLabel.text = symbol;

        Label textLabel = new();
        titleContainer.Add(textLabel);
        textLabel.text = text;
    }
}
