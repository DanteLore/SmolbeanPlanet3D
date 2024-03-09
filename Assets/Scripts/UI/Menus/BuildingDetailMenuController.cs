using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingDetailsMenuController : SmolbeanMenu
{
    private UIDocument document;
    private BuildManager buildManager;
    private GridManager gridManager;
    private SoundPlayer soundPlayer;
    private Transform target;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        buildManager = FindFirstObjectByType<BuildManager>();
        gridManager = FindFirstObjectByType<GridManager>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        InvokeRepeating(nameof(Refresh), 0.1f, 0.1f);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(Refresh));
    }

    private void Refresh()
    {
        if (ReferenceEquals(buildManager.EditTargetTransform, target))
            return;

        target = buildManager.EditTargetTransform;

        Clear();

        if (target != null)
            SetTarget();
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        buildManager.ClearSelection();
        MenuController.Instance.CloseAll();
    }

    private void SetTarget()
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
            var inventoryContainer = new VisualElement();
            inventoryContainer.AddToClassList("inventoryContainer");
            mainScrollView.Add(inventoryContainer);

            Label inventoryLabel = new();
            inventoryContainer.Add(inventoryLabel);
            inventoryLabel.text = "𐚱";

            foreach (var item in building.Inventory.Totals)
            {
                Button button = new();
                button.userData = item.dropSpec;
                button.text = item.quantity.ToString();
                button.style.backgroundColor = new Color(0, 0, 0, 0);
                button.style.backgroundImage = item.dropSpec.thumbnail;
                inventoryContainer.Add(button);
            }
        }
    }

    private static void BuildIngredients(SmolbeanBuilding building, ScrollView mainScrollView)
    {
        var ingredientContainer = new VisualElement();
        ingredientContainer.AddToClassList("ingredientContainer");
        mainScrollView.Add(ingredientContainer);

        Label ingredientsLabel = new();
        ingredientContainer.Add(ingredientsLabel);
        ingredientsLabel.text = "⚒";

        foreach (var ingredient in building.BuildingSpec.ingredients)
        {
            Button button = new();
            button.userData = ingredient.item;
            button.text = ingredient.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = ingredient.item.thumbnail;
            ingredientContainer.Add(button);
        }
    }

    private void Clear()
    {
        document.rootVisualElement.Q<ScrollView>("mainScrollView").Clear();
    }
}
