using System.Linq;
using Mono.Cecil;
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
        if (target)
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

        BuildIngredients();
        BuildInventory();
        BuildRecipe();
        BuildJobs();
        BuildResidents();
    }

    private void BuildResidents()
    {
        var home = building.GetComponent<SmolbeanHome>();

        bool anythingToShow = home != null && home.Colonists.Count() > 0;
        SetTabVisibility(root.Q<TabView>("mainTabView"), "residentsTab", anythingToShow);

        if (!anythingToShow)
            return;

        var residentsListView = root.Q<MultiColumnListView>("residentsListView");
        ColonistViewBuilder.BuildColonistsView(residentsListView, home.Colonists.ToArray());

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

    private void BuildRecipe()
    {
        if (building is not FactoryBuilding)
            return;
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
        bool anythingToShow = !building.Inventory.IsEmpty();
        SetTabVisibility(root.Q<TabView>("mainTabView"), "inventoryTab", anythingToShow);

        if (!anythingToShow)
            return;

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

    private void BuildJobs()
    {
        var jobs = JobController.Instance.GetAllJobsForBuilding(building).ToArray();

        bool anythingToShow = jobs.Length > 0;
        SetTabVisibility(root.Q<TabView>("mainTabView"), "jobsTab", anythingToShow);

        if (!anythingToShow)
            return;

        var jobsListView = root.Q<MultiColumnListView>("jobsListView");
        JobViewBuilder.BuildJobView(jobsListView, jobs);
    }

    private void BuildIngredients()
    {
        if (building.IsComplete)
            return;

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

    private static void SetTabVisibility(TabView tabView, string tabName, bool visible)
    {
        // There should be a better way!!

        // Find the index of the named tab in the list
        var tabs = tabView.Query<Tab>().ToList();
        var theTab = tabs.First(t => t.name == tabName);
        int tabIndex = tabs.IndexOf(theTab);

        // Use that index to find the matching header, hide both header and tab
        var headers = tabView.Query<VisualElement>("unity-tab__header").ToList();
        var theHeader = headers[tabIndex];

        if (visible && theTab.style.display == DisplayStyle.None)
        {
            // Tab being made visible
            theTab.style.display = StyleKeyword.Null;
            theHeader.style.display = DisplayStyle.Flex;
        }
        else if (!visible && theTab.style.display != DisplayStyle.None)
        {
            // Tab being hidden
            theTab.style.display = DisplayStyle.None;
            theHeader.style.display = DisplayStyle.None;

            // If we're hiding the selected tab, select a different one!
            if (tabIndex == tabView.selectedTabIndex)
            {
                var firstVisibleTab = tabs.FirstOrDefault(t => t.style.display != DisplayStyle.None);
                tabView.selectedTabIndex = firstVisibleTab != null ? tabs.IndexOf(firstVisibleTab) : 0;
            }
        }
    }
}
