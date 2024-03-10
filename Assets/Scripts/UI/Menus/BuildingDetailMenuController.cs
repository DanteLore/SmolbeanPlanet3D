using UnityEngine;
using UnityEngine.UIElements;

public class BuildingDetailsMenuController : BaseDetailsMenuController
{
    private BuildManager buildManager;

    protected override void OnEnable()
    {
        buildManager = FindFirstObjectByType<BuildManager>();

        base.OnEnable();
    }

    protected override void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        buildManager.ClearSelection();
        MenuController.Instance.CloseAll();
    }

    protected override void Refresh()
    {
        if (ReferenceEquals(buildManager.EditTargetTransform, target))
            return;

        target = buildManager.EditTargetTransform;

        Clear();

        if (target != null)
            DrawMenu();
    }

    private void DrawMenu()
    {
        var building = target.GetComponent<SmolbeanBuilding>();

        var buildingImage = document.rootVisualElement.Q<VisualElement>("buildingImage");
        buildingImage.style.backgroundImage = building.BuildingSpec.thumbnail;

        var pos = gridManager.GetGameSquareFromWorldCoords(building.transform.position);
        var positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
        positionLabel.text = $"{pos.x}λ \u00d7 {pos.y}φ";

        var mainScrollView = document.rootVisualElement.Q<ScrollView>("mainScrollView");

        if (!building.IsComplete)
        {
            BuildIngredients(building, mainScrollView);
        }

        if(!building.Inventory.IsEmpty())
        {
            BuildInventory(building, mainScrollView);
        }

        if (building is FactoryBuilding factory)
        {
            BuildRecipe(mainScrollView, factory);
        }
    }

    private static void BuildRecipe(ScrollView mainScrollView, FactoryBuilding factory)
    {
        Label recipeLabel = new();
        recipeLabel.AddToClassList("notoSansSymbols");
        recipeLabel.AddToClassList("bigLabel");
        mainScrollView.Add(recipeLabel);
        recipeLabel.text = "⎌";

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
        Label inventoryLabel = new();
        inventoryLabel.AddToClassList("notoLinearA");
        inventoryLabel.AddToClassList("bigLabel");
        mainScrollView.Add(inventoryLabel);
        inventoryLabel.text = "𐚱";

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

    private static void BuildIngredients(SmolbeanBuilding building, ScrollView mainScrollView)
    {
        Label ingredientsLabel = new();
        ingredientsLabel.AddToClassList("notoEmoji");
        ingredientsLabel.AddToClassList("bigLabel");
        mainScrollView.Add(ingredientsLabel);
        ingredientsLabel.text = "⚒";

        var ingredientContainer = new VisualElement();
        ingredientContainer.AddToClassList("ingredientContainer");
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
}
