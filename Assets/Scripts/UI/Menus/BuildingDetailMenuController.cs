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
    }

    private void BuildRecipe()
    {
        bool anythingToShow = building is FactoryBuilding;
        SetTabVisibility(root.Q<TabView>("mainTabView"), "recipeTab", anythingToShow);

        if (!anythingToShow)
            return;

        var recipeContainer = root.Q<VisualElement>("recipeContainer");
        recipeContainer.Clear();

        var factory = (FactoryBuilding)building;

        foreach (var ingredient in factory.recipe.ingredients)
        {
            Button button = new();
            button.text = ingredient.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = ingredient.item.thumbnail;
            recipeContainer.Add(button);
        }

        Label equalsLabel = new();
        equalsLabel.AddToClassList("recipeEqualsArrow");
        recipeContainer.Add(equalsLabel);
        equalsLabel.text = "→";

        Button productButton = new();
        productButton.text = factory.recipe.quantity.ToString();
        productButton.style.backgroundImage = factory.recipe.createdItem.thumbnail;
        recipeContainer.Add(productButton);
    }

    private void BuildInventory()
    {
        bool anythingToShow = building.IsComplete;
        SetTabVisibility(root.Q<TabView>("mainTabView"), "inventoryTab", anythingToShow);

        if (!anythingToShow)
            return;

        var inventoryContainer = root.Q<VisualElement>("inventoryContainer");
        inventoryContainer.Clear();

        if (building.Inventory.IsEmpty())
        {
            Label emptyLabel = new("Empty");
            inventoryContainer.Add(emptyLabel);
            return;
        }

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
        bool anythingToShow = !building.IsComplete;
        SetTabVisibility(root.Q<TabView>("mainTabView"), "buildTab", anythingToShow);

        if (!anythingToShow)
            return;

        var buildContainer = root.Q<VisualElement>("buildContainer");
        buildContainer.Clear();

        foreach (var ingredient in building.BuildingSpec.ingredients)
        {
            int required = ingredient.quantity;
            int delivered = building.Inventory.ItemCount(ingredient.item);

            Button button = new();
            button.text = $"{delivered}/{required}";
            button.style.backgroundImage = ingredient.item.thumbnail;
            buildContainer.Add(button);
        }
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

            if (tabIndex >= tabView.selectedTabIndex)
                tabView.selectedTabIndex++;
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
