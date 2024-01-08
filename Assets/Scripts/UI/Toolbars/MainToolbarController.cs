using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainToolbarController : MonoBehaviour
{
    UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var buildToolbarButton = document.rootVisualElement.Q<Button>("buildToolbarButton");
        buildToolbarButton.clicked += BuildToolbarButtonClicked;
        
        var mapButton = document.rootVisualElement.Q<Button>("mapButton");
        mapButton.clicked += MapButtonClicked;
        
        var inventoryButton = document.rootVisualElement.Q<Button>("inventoryButton");
        inventoryButton.clicked += InventoryButtonClicked;
    }

    private void BuildToolbarButtonClicked()
    {
        ToolbarController.Instance.ShowToolbar("BuildToolbar");
    }

    private void MapButtonClicked()
    {
        MenuController.Instance.ShowMenu("MapMenu");
    }

    private void InventoryButtonClicked()
    {
        MenuController.Instance.ShowMenu("InventoryMenu");
    }
}
