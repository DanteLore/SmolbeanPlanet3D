using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildToolbarController : MonoBehaviour
{
    UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainToolbarButton");
        mainMenuButton.clicked += MainMenuButtonClicked;
        
        var woodcutterButton = document.rootVisualElement.Q<Button>("woodcutterButton");
        woodcutterButton.clicked += WoodcutterButtonClicked;
    }

    private void MainMenuButtonClicked()
    {
        ToolbarController.Instance.ShowToolbar();
    }

    private void WoodcutterButtonClicked()
    {
        //BuildManager.Instance.BeginBuild();
    }
}
