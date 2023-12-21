using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class DeliveryRequestsMenu : SmolbeanMenu
{
    private UIDocument document;

    VisualElement listContainer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;
        
        listContainer = document.rootVisualElement.Q<VisualElement>("listContainer");

        InvokeRepeating("RefreshList", 0.1f, 2.0f);
    }

    void OnDisable()
    {
        CancelInvoke("RefreshList");
    }

    private void RefreshList()
    {
        listContainer.Clear();

        foreach(var request in DeliveryManager.Instance.ClaimedDeliveryRequests)
            AddRow(request, false);
            
        foreach(var request in DeliveryManager.Instance.UnclaimedDeliveryRequests)
            AddRow(request, true);
    }

    private void AddRow(DeliveryRequest request, bool waiting)
    {
        var row = new VisualElement();
        row.AddToClassList("deliveryRow");
        listContainer.Add(row);

        row.Add(new Label { text = $"P{request.Priority}" });

        var buildingIcon = new Button();
        buildingIcon.AddToClassList("buildingIcon");
        buildingIcon.style.backgroundColor = new Color(0, 0, 0, 0);
        buildingIcon.style.backgroundImage = request.Building.IsComplete ? request.Building.BuildingSpec.thumbnail : request.Building.BuildingSpec.siteThumbnail;
        row.Add(buildingIcon);

        row.Add(new Label { text = request.Building.name });

        var resourceIcon = new Button();
        resourceIcon.AddToClassList("resourceIcon");
        resourceIcon.style.backgroundColor = new Color(0, 0, 0, 0);
        resourceIcon.style.backgroundImage = request.Item.thumbnail;
        row.Add(resourceIcon);

        row.Add(new Label { text = request.Item.dropName });

        row.Add(new Label { text = $"x {request.Quantity}" });

        row.Add(new Label { text = waiting ? "Pending" : "In transit" });
    }

    private void CloseButtonClicked()
    {
        MenuController.Instance.CloseAll();
    }
}
