using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        var soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        series = DataCollectionManager.Instance.Series.ToList();
        groups = series.Select(s => s.seriesGroup).Distinct().OrderBy(s => s.ToLower()).ToList();

        document = GetComponent<UIDocument>();
        document.rootVisualElement.Q<Button>("closeButton").clicked += () =>
        {
            soundPlayer.Play("Click");
            MenuController.Instance.CloseAll();
        };
   
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

        UpdateKey();
    }

    private void GroupSelected(ChangeEvent<string> evt)
    {
        selectedGroupIndex = groupSelect.index;
        chart.Series = series.Where(s => s.seriesGroup == groups[selectedGroupIndex]).ToArray();

        UpdateKey();
    }

    private void UpdateKey()
    {
        var keyContainer = document.rootVisualElement.Q<VisualElement>("keyContainer");
        keyContainer.Clear();

        foreach(var series in chart.Series)
        {
            var item = new VisualElement();
            item.AddToClassList("chartKeyItemContainer");
            keyContainer.Add(item);

            var colorButton = new Button();
            colorButton.AddToClassList("chartKeyColorButton");
            colorButton.style.backgroundColor = series.IsVisible ? series.lineColor : new StyleColor(Color.grey);
            colorButton.clicked += () => {
                series.ToggleVisibility();
                colorButton.style.backgroundColor = series.IsVisible ? series.lineColor : new StyleColor(Color.grey);
            };
            item.Add(colorButton);

            var label = new Label(series.seriesName);
            item.Add(label);
        }
    }
}
