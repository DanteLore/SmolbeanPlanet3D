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
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainMenuButton");
        mainMenuButton.clicked += MainMenuButtonClicked;
        
        var buildToolbarButton = document.rootVisualElement.Q<Button>("buildToolbarButton");
        buildToolbarButton.clicked += BuildToolbarButtonClicked;
        
        var mapButton = document.rootVisualElement.Q<Button>("mapButton");
        mapButton.clicked += MapButtonClicked;
    }

    private void MainMenuButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }

    private void BuildToolbarButtonClicked()
    {
        ToolbarController.Instance.ShowToolbar("BuildToolbar");
    }

    private void MapButtonClicked()
    {
        MenuController.Instance.ShowMenu("MapMenu");
    }
}
