using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

public class SaveGameMenuController : SmolbeanMenu
{
    UIDocument document;
    private SoundPlayer soundPlayer;
    private TextField filenameTextField;
    private ListView fileListView;
    private string[] files;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        files = SaveGameManager.Instance.ListSaveFiles().ToArray();
        
        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;
        
        var saveGameButton = document.rootVisualElement.Q<Button>("saveGameButton");
        saveGameButton.clicked += SaveButtonClicked;
        
        filenameTextField = document.rootVisualElement.Q<TextField>("filenameTextField");
        filenameTextField.value = SaveGameNameGenerator.Generate();
        filenameTextField.RegisterValueChangedCallback(FilenameValueChanged);

        fileListView = document.rootVisualElement.Q<ListView>("fileListView");
        fileListView.itemsSource = files;
        fileListView.makeItem = () => new Label();
        fileListView.bindItem = (e, i) => (e as Label).text = files[i];
        fileListView.selectionType = SelectionType.Single;
        fileListView.focusable = true;
        fileListView.selectionChanged += FileSelectedFromList;
        fileListView.itemsChosen += FileChosenFromList;  // Both methods need to be hooked up, or neither works :facepalm:
    }

    private void FilenameValueChanged(ChangeEvent<string> evt)
    {
        if (files.Contains(evt.newValue))
            fileListView.SetSelection(Array.IndexOf(files, evt.newValue));
        else
            fileListView.ClearSelection();
    }

    private void CancelButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.ShowMenu();
    }

    private void SaveButtonClicked()
    {
        soundPlayer.Play("Click");
        SaveGameManager.Instance.SaveGame(filenameTextField.value);
        MenuController.Instance.ShowMenu();
    }

    private void FileSelectedFromList(IEnumerable<object> items)
    {
        if (fileListView.selectedItem != null)
        {
            filenameTextField.value = fileListView.selectedItem.ToString();
        }
    }

    private void FileChosenFromList(IEnumerable<object> items)
    {
        if (fileListView.selectedItem != null)
        {
            soundPlayer.Play("Click");
            filenameTextField.value = fileListView.selectedItem.ToString();
            SaveButtonClicked();
        }
    }
}
