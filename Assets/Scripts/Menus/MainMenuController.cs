using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var newGameButton = document.rootVisualElement.Q<Button>("newGameButton");
        newGameButton.clicked += NewGameButtonClicked;
        
        var startGameButton = document.rootVisualElement.Q<Button>("resumeButton");
        startGameButton.clicked += ResumeButtonClicked;
        
        var quitButton = document.rootVisualElement.Q<Button>("quitButton");
        quitButton.clicked += QuitButtonClicked;
    }

    private void NewGameButtonClicked()
    {
        MenuController.Instance.ShowMenu("NewGameMenu");
    }

    private void ResumeButtonClicked()
    {
        MenuController.Instance.CloseAll();
    }

    private void QuitButtonClicked()
    {
        Debug.Log("Quittin' time, folks");
        Application.Quit(0);
    }
}
