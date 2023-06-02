using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildToolbarController : MonoBehaviour
{
    private UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainToolbarButton");
        mainMenuButton.clicked += MainMenuButtonClicked;
        
        var buttonContainer = document.rootVisualElement.Q<VisualElement>("buildingButtonContainer");
        buttonContainer.Clear();

        foreach(var spec in BuildManager.Instance.buildings)
        {
            Button button = new Button();
            button.clickable.clickedWithEventInfo += BuildButtonClicked;
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = spec.thumbnail;
            button.userData = spec;
            buttonContainer.Add(button);
        }
    }

    private void DeleteButtonClicked()
    {

    }

    private void RotateButtonClicked()
    {
        
    }

    private void BuildButtonClicked(EventBase eventBase)
    {
        var spec = (BuildingSpec)((Button)eventBase.target).userData;
        BuildManager.Instance.BeginBuild(spec);
    }

    private void MainMenuButtonClicked()
    {
        BuildManager.Instance.EndBuild();
        ToolbarController.Instance.ShowToolbar();
    }
}
