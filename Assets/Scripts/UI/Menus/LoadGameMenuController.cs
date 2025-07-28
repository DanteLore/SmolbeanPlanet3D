using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class LoadGameMenuController : SmolbeanMenu
{
    UIDocument document;
    private SoundPlayer soundPlayer;
    private MultiColumnListView fileListView;
    private SaveFileViewModel[] files;
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

        fileListView = document.rootVisualElement.Q<MultiColumnListView>("fileListView");
        SaveFileListViewModelBuilder.BuildFileView(fileListView, files.ToList());

        fileListView.selectionChanged += FileSelectedFromList;
        fileListView.itemsChosen += FileChosenFromList; // Both methods need to be hooked up, or neither works :facepalm:
    }

    private void LoadButtonClicked()
    {
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
        yield return SaveGameManager.Instance.LoadGame(fileListView.selectedItem.ToString());
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
