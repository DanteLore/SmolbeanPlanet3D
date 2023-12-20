using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class DeliveryRequestsMenu : SmolbeanMenu
{
    private UIDocument document;
    private string[] files;

    VisualElement buildingListContainer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;
        
        buildingListContainer = document.rootVisualElement.Q<VisualElement>("buildingListContainer");

        InvokeRepeating("RefreshBuildingsList", 0.1f, 2.0f);
    }

    void OnDisable()
    {
        CancelInvoke("RefreshBuildingsList");
    }

    private void RefreshBuildingsList()
    {
        buildingListContainer.Clear();

        var buildings = BuildManager.Instance.Buildings.Where(b => b.Inventory.Count > 0).OrderByDescending(b => b.Inventory.Count);

        foreach(var building in buildings)
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

            foreach(var item in building.Inventory.Totals.OrderBy(i => i.dropSpec.dropName))
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
        MenuController.Instance.CloseAll();
    }
}
