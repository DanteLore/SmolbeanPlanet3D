using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class GraphMenuController : SmolbeanMenu
{
    private UIDocument document;
    private LineChart chart;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        document.rootVisualElement.Q<Button>("closeButton").clicked += () => MenuController.Instance.CloseAll();
   
        var graphBox = document.rootVisualElement.Q<VisualElement>("graphBox");

        var seriesSelect = document.rootVisualElement.Q<DropdownField>("seriesDropdown");
        seriesSelect.choices = DataCollectionManager.Instance.Series.Select(s => s.displayName).ToList();
        seriesSelect.index = 0;
        seriesSelect.RegisterValueChangedCallback(evt => { SeriesSelected(seriesSelect.index); });

        chart = new LineChart
        {
            Series = DataCollectionManager.Instance.Series[0]
        };
        graphBox.Add(chart);
    }

    private void SeriesSelected(int index)
    {
        chart.Series = DataCollectionManager.Instance.Series[index];
    }
}
