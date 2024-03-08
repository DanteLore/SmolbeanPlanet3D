using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class LoadGameMenuController : SmolbeanMenu
{
    UIDocument document;
    private SoundPlayer soundPlayer;
    private ListView fileListView;
    private string[] files;
    private Button loadGameButton;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        files = SaveGameManager.Instance.ListSaveFiles().ToArray();
        
        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;
        
        loadGameButton = document.rootVisualElement.Q<Button>("loadGameButton");
        loadGameButton.clicked += LoadButtonClicked;
        loadGameButton.SetEnabled(false); 

        fileListView = document.rootVisualElement.Q<ListView>("fileListView");
        fileListView.itemsSource = files;
        fileListView.makeItem = () => new Label();
        fileListView.bindItem = (e, i) => (e as Label).text = files[i];
        fileListView.selectionType = SelectionType.Single;
        fileListView.selectionChanged += FileSelectedFromList;
        fileListView.itemsChosen += FileChosenFromList; // Both methods need to be hooked up, or neither works :facepalm:
    }

    private void LoadButtonClicked()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        soundPlayer.Play("Click");
        yield return null;
        document.rootVisualElement.style.display = DisplayStyle.None;
        yield return SaveGameManager.Instance.LoadGame((string)fileListView.selectedItem);
        yield return null;
        GameStateManager.Instance.StartGame();
        MenuController.Instance.CloseAll();
    }

    private void CancelButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu();
    }

    private void FileSelectedFromList(IEnumerable<object> items)
    {
         loadGameButton.SetEnabled(fileListView.selectedItem != null);
    }

    private void FileChosenFromList(IEnumerable<object> items)
    {
        if(fileListView.selectedItem != null)
        {
            soundPlayer.Play("Click");
            LoadButtonClicked();
        }
    }
}
