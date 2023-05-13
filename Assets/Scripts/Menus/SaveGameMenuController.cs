using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

public class SaveGameMenuController : MonoBehaviour
{
    UIDocument document;
    private TextField filenameTextField;
    private ListView fileListView;
    private string[] files;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();

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
        fileListView.selectionChanged += FileSelectedFromList;
    }

    private void FilenameValueChanged(ChangeEvent<string> evt)
    {
        if(files.Contains(evt.newValue))
            fileListView.SetSelection(Array.IndexOf(files, evt.newValue));
        else
            fileListView.ClearSelection();
    }

    private void CancelButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }

    private void SaveButtonClicked()
    {
        SaveGameManager.Instance.SaveGame(filenameTextField.value);
        MenuController.Instance.ShowMenu();
    }

    private void FileSelectedFromList(IEnumerable<object> items)
    {
        Debug.Log("Hello " + fileListView.selectedItem);
        if(fileListView.selectedItem != null)
        {
            filenameTextField.value = fileListView.selectedItem.ToString();
        }
    }
}
