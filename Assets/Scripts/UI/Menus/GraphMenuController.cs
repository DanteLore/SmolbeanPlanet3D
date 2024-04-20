using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class GraphMenuController : SmolbeanMenu
{
    private UIDocument document;
    private LineChart chart;
    private int selectedGroupIndex = 0;
    private List<DataCollectionSeries> series;
    private List<string> groups;
    private DropdownField groupSelect;

    void OnEnable()
    {
        series = DataCollectionManager.Instance.Series.ToList();
        groups = series.Select(s => s.seriesGroup).Distinct().OrderBy(s => s.ToLower()).ToList();

        document = GetComponent<UIDocument>();
        document.rootVisualElement.Q<Button>("closeButton").clicked += () => MenuController.Instance.CloseAll();
   
        var graphBox = document.rootVisualElement.Q<VisualElement>("graphBox");

        groupSelect = document.rootVisualElement.Q<DropdownField>("groupDropdown");
        groupSelect.choices = groups;
        groupSelect.index = selectedGroupIndex;
        groupSelect.RegisterValueChangedCallback(GroupSelected);

        chart = new LineChart
        {
            Series = series.Where(s => s.seriesGroup == groups[selectedGroupIndex]).ToArray()
        };
        graphBox.Add(chart);
    }

    private void GroupSelected(ChangeEvent<string> evt)
    {
        selectedGroupIndex = groupSelect.index;
        chart.Series = series.Where(s => s.seriesGroup == groups[selectedGroupIndex]).ToArray();
    }
}
