using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : SmolbeanMenu
{
    UIDocument document;
    private SoundPlayer soundPlayer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var newGameButton = document.rootVisualElement.Q<Button>("newGameButton");
        newGameButton.clicked += NewGameButtonClicked;
        
        var resumeButton = document.rootVisualElement.Q<Button>("resumeButton");
        resumeButton.clicked += ResumeButtonClicked;
        resumeButton.visible = GameStateManager.Instance.IsStarted;
        GameStateManager.Instance.GameStatusChanged += (o, started) => resumeButton.visible = started;
        
        var saveGameButton = document.rootVisualElement.Q<Button>("saveGameButton");
        saveGameButton.clicked += SaveGameButtonClicked;
        saveGameButton.visible = GameStateManager.Instance.IsStarted;
        GameStateManager.Instance.GameStatusChanged += (o, started) => saveGameButton.visible = started;

        var loadGameButton = document.rootVisualElement.Q<Button>("loadGameButton");
        loadGameButton.clicked += LoadGameButtonClicked;
        
        var settingsButton = document.rootVisualElement.Q<Button>("settingsButton");
        settingsButton.clicked += SettingsButtonClicked;
        
        var quitButton = document.rootVisualElement.Q<Button>("quitButton");
        quitButton.clicked += QuitButtonClicked;
    }

    private void SettingsButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu("SettingsMenu");
    }

    private void LoadGameButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu("LoadGameMenu");
    }

    private void SaveGameButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu("SaveGameMenu");
    }

    private void NewGameButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu("NewGameMenu");
    }

    private void ResumeButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }

    private void QuitButtonClicked()
    {
        soundPlayer.Play("Click");
        Debug.Log("Quittin' time, folks");
        Application.Quit(0);
    }
}
