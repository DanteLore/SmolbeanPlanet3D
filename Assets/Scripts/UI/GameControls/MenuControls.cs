using UnityEngine;
using UnityEngine.UIElements;

public class menuControls : MonoBehaviour
{    
    void OnEnable()
    {
        var document = GetComponent<UIDocument>();
        var soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainMenuButton");
        mainMenuButton.clicked += () => {
            soundPlayer.Play("Click");
            MenuController.Instance.ShowMenu();
        };
    }
}
