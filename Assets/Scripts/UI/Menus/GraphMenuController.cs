using UnityEngine;
using UnityEngine.UIElements;

public class GraphMenuController : SmolbeanMenu
{
    private UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        document.rootVisualElement.Q<Button>("closeButton").clicked += () => MenuController.Instance.CloseAll();

        var graphBox = document.rootVisualElement.Q<VisualElement>("graphBox");

        var graph = new LineChart();
        graphBox.Add(graph);
    }
}
