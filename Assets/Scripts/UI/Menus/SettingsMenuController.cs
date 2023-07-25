using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;

public class SettingsMenuController : MonoBehaviour
{
    UIDocument document;
    private string[] files;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();

        files = SaveGameManager.Instance.ListSaveFiles().ToArray();
        
        var doneButton = document.rootVisualElement.Q<Button>("doneButton");
        doneButton.clicked += DoneButtonClicked;
    }

    private void DoneButtonClicked()
    {
        MenuController.Instance.ShowMenu();
    }
}
