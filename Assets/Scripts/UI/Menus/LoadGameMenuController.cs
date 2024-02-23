using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System;

public class LoadGameMenuController : SmolbeanMenu
{
    UIDocument document;
    private ListView fileListView;
    private string[] files;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();

        files = SaveGameManager.Instance.ListSaveFiles().ToArray();
        
        var cancelButton = document.rootVisualElement.Q<Button>("cancelButton");
        cancelButton.clicked += CancelButtonClicked;
     
        fileListView = document.rootVisualElement.Q<ListView>("fileListView");
        fileListView.itemsSource = files;
        fileListView.makeItem = () => new Button();
        fileListView.bindItem = (e, i) =>
        {
            string filename = files[i];
            (e as Button).text = filename;
            (e as Button).clicked += () => LoadButtonClicked(filename);

        };
        fileListView.selectionType = SelectionType.None;
    }

    private void LoadButtonClicked(string filename)
    {
        SaveGameManager.Instance.LoadGame(filename);
        MenuController.Instance.CloseAll();
    }

    private void CancelButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }
}
