using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : SmolbeanMenu
{
    UIDocument document;
    private SoundPlayer soundPlayer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var newGameButton = root.Q<Button>("newGameButton");
        newGameButton.clicked += NewGameButtonClicked;

        var resumeButton = root.Q<Button>("resumeButton");
        resumeButton.clicked += ResumeButtonClicked;
        resumeButton.visible = CanResume();
        GameStateManager.Instance.GameStatusChanged += (o, started) => resumeButton.visible = CanResume();

        var saveGameButton = root.Q<Button>("saveGameButton");
        saveGameButton.clicked += SaveGameButtonClicked;
        saveGameButton.visible = GameStateManager.Instance.IsStarted;
        GameStateManager.Instance.GameStatusChanged += (o, started) => saveGameButton.visible = started;

        var loadGameButton = root.Q<Button>("loadGameButton");
        loadGameButton.clicked += LoadGameButtonClicked;

        var settingsButton = root.Q<Button>("settingsButton");
        settingsButton.clicked += SettingsButtonClicked;

        var quitButton = root.Q<Button>("quitButton");
        quitButton.clicked += QuitButtonClicked;

        root.Q<Label>("debugInfoLabel").text = GetDisplayDebugInfo();
    }

    private static string GetDisplayDebugInfo()
    {
        var sb = new StringBuilder();
        sb.Append($"{BuildInfo.ProductName} v{BuildInfo.Version}");
        sb.Append($" (Unity {BuildInfo.UnityVersion}, {BuildInfo.Platform}/{BuildInfo.ScriptingBackend})");
        sb.Append($" built at {BuildInfo.BuildTimeUtc}");
        return sb.ToString();
    }

    private static bool CanResume()
    {
        return 
            GameStateManager.Instance.IsStarted ||
            (
                !string.IsNullOrEmpty(PrefsManager.Instance.LastSaveName) &&
                SaveGameManager.Instance.SaveFileExists(PrefsManager.Instance.LastSaveName)
            );
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

        if(GameStateManager.Instance.IsStarted)
            MenuController.Instance.CloseAll();
        else    
            StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
        yield return SaveGameManager.Instance.LoadGame(PrefsManager.Instance.LastSaveName);
        yield return null;
        GameStateManager.Instance.StartGame();
        MenuController.Instance.CloseAll();
    }

    private void QuitButtonClicked()
    {
        soundPlayer.Play("Click");
        Debug.Log("Quittin' time, folks");
        Application.Quit(0);
    }
}
