using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryMenu : MonoBehaviour
{
    private UIDocument document;
    private string[] files;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;
    }

    private void CloseButtonClicked()
    {
        MenuController.Instance.CloseAll();
    }
}
