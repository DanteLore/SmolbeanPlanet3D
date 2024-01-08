using UnityEngine;
using UnityEngine.UIElements;

public class menuControls : MonoBehaviour
{    
    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainMenuButton");
        mainMenuButton.clicked += () => {
            Debug.Log("w00t");
            MenuController.Instance.ShowMenu();
        };
    }
}
