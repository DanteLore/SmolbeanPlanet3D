using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class LoadGameMenuController : MonoBehaviour
{
    UIDocument document;
    private ListView fileListView;
    private string[] files;
    private Button loadGameButton;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();

        files = SaveGameManager.Instance.ListSaveFiles().ToArray();
        
        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;
        
        loadGameButton = document.rootVisualElement.Q<Button>("loadGameButton");
        loadGameButton.clicked += LoadButtonClicked;

        fileListView = document.rootVisualElement.Q<ListView>("fileListView");
        fileListView.itemsSource = files;
        fileListView.makeItem = () => new Label();
        fileListView.bindItem = (e, i) => (e as Label).text = files[i];
        fileListView.selectionType = SelectionType.Single;
        fileListView.selectionChanged += FileSelectedFromList;
    }

    private void LoadButtonClicked()
    {
        SaveGameManager.LoadGame((string)fileListView.selectedItem);
        MenuController.Instance.ShowMenu();
    }

    private void CancelButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }

    private void FileSelectedFromList(IEnumerable<object> items)
    {
         loadGameButton.SetEnabled(fileListView.selectedItem != null);
    }
}
