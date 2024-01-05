using UnityEngine;
using UnityEngine.UIElements;

public class DebugControls : MonoBehaviour
{
    private float deltaTime;

    private Label fpsLabel;
    
    void Start()
    {
        var document = GetComponent<UIDocument>();
        
        fpsLabel = document.rootVisualElement.Q<Label>("fpsLabel");
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = Time.timeScale / deltaTime;
        fpsLabel.text = Mathf.Ceil(fps).ToString() + " fps";
    }
}
