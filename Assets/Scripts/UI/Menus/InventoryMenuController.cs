using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class InventoryMenu : SmolbeanMenu
{
    private UIDocument document;
    private SoundPlayer soundPlayer;

    VisualElement buildingListContainer;
    private DropdownField modeDropdown;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        modeDropdown = document.rootVisualElement.Q<DropdownField>("modeDropdown");
        modeDropdown.RegisterValueChangedCallback(ModeChanged);

        buildingListContainer = document.rootVisualElement.Q<VisualElement>("buildingScroller");

        InvokeRepeating(nameof(RefreshBuildingsList), 2.0f, 2.0f);
        RefreshBuildingsList();
    }

    private void ModeChanged(ChangeEvent<string> evt)
    {
        RefreshBuildingsList();
    }

    void OnDisable()
    {
        CancelInvoke(nameof(RefreshBuildingsList));
    }

    private void RefreshBuildingsList()
    {
        buildingListContainer.Clear();

        var buildings = BuildingController.Instance.Buildings.Where(b => b.Inventory.Count > 0).OrderByDescending(b => b.Inventory.Count);

        if (modeDropdown.index == 0)
            ShowInventoryByBuilding(buildings);
        else
            ShowInventoryTotals(buildings);
    }

    private void ShowInventoryTotals(IOrderedEnumerable<SmolbeanBuilding> buildings)
    {
        Inventory totals = new();

        foreach (var building in buildings)
            foreach (var item in building.Inventory.Totals)
                totals.PickUp(item);

        var inventoryContainer = new VisualElement();
        inventoryContainer.AddToClassList("inventoryContainer");
        buildingListContainer.Add(inventoryContainer);

        foreach (var item in totals.Totals.OrderBy(i => i.dropSpec.dropName))
        {
            Button button = new Button();
            button.text = item.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = item.dropSpec.thumbnail;
            button.userData = item.dropSpec;
            inventoryContainer.Add(button);
        }
    }

    private void ShowInventoryByBuilding(IOrderedEnumerable<SmolbeanBuilding> buildings)
    {
        foreach (var building in buildings)
        {
            var buildingRow = new VisualElement();
            buildingRow.AddToClassList("buildingRow");
            buildingListContainer.Add(buildingRow);

            var buildingHeader = new VisualElement();
            buildingHeader.AddToClassList("buildingHeader");
            buildingRow.Add(buildingHeader);

            var buildingIcon = new Button();
            buildingHeader.AddToClassList("buildingIcon");
            buildingIcon.style.backgroundColor = new Color(0, 0, 0, 0);
            buildingIcon.style.backgroundImage = building.IsComplete ? building.BuildingSpec.thumbnail : building.BuildingSpec.siteThumbnail;
            buildingHeader.Add(buildingIcon);

            var buildingTitle = new Label();
            buildingTitle.text = building.name;
            buildingHeader.Add(buildingTitle);

            var inventoryContainer = new VisualElement();
            inventoryContainer.AddToClassList("inventoryContainer");
            buildingRow.Add(inventoryContainer);

            foreach (var item in building.Inventory.Totals.OrderBy(i => i.dropSpec.dropName))
            {
                Button button = new Button();
                button.text = item.quantity.ToString();
                button.style.backgroundColor = new Color(0, 0, 0, 0);
                button.style.backgroundImage = item.dropSpec.thumbnail;
                button.userData = item.dropSpec;
                inventoryContainer.Add(button);
            }
        }
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }
}
