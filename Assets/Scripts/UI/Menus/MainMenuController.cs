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
        
        var saveGameButton = document.rootVisualElement.Q<Button>("saveGameButton");
        saveGameButton.clicked += SaveGameButtonClicked;
        
        var loadGameButton = document.rootVisualElement.Q<Button>("loadGameButton");
        loadGameButton.clicked += LoadGameButtonClicked;
        
        var settingsButton = document.rootVisualElement.Q<Button>("settingsButton");
        settingsButton.clicked += SettingsButtonClicked;
        
        var quitButton = document.rootVisualElement.Q<Button>("quitButton");
        quitButton.clicked += QuitButtonClicked;
    }

    private void SettingsButtonClicked()
    {
        MenuController.Instance.ShowMenu("SettingsMenu");
    }

    private void LoadGameButtonClicked()
    {
        MenuController.Instance.ShowMenu("LoadGameMenu");
    }

    private void SaveGameButtonClicked()
    {
        MenuController.Instance.ShowMenu("SaveGameMenu");
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
