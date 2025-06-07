using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class ColonistViewBuilder
{
    public static void BuildColonistsView(MultiColumnListView colonistsListView, SmolbeanColonist[] colonists)
    {
        var colonistsVM = colonists.Select(c => new ColonistViewModel(c)).ToList();
        
        // COLONIST
        var colonistColumn = colonistsListView.columns.First(c => c.name == "colonistNameColumn");
        if (colonistColumn != null)
        {
            colonistColumn.makeCell = () =>
            {
                var container = new VisualElement();
                container.AddToClassList("colonists-thumb-and-label-row");

                var icon = new Image() { name = "jobRowThumbnail" };
                icon.AddToClassList("colonists-thumb-image");
                container.Add(icon);

                var lbl = new Label { name = "valueLabel" };
                lbl.AddToClassList("colonists-table-label");
                container.Add(lbl);

                return container;
            };
            colonistColumn.bindCell = (cell, i) =>
            {
                var jobsList = (List<ColonistViewModel>)colonistsListView.itemsSource;
                var m = jobsList[i];
                cell.Q<Label>("valueLabel").text = jobsList[i].ColonistName;
                cell.Q<Image>("jobRowThumbnail").image = jobsList[i].ColonistThumbnail;
            };
            colonistColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<ColonistViewModel>)colonistsListView.itemsSource)[rowA].ColonistName,
                    ((List<ColonistViewModel>)colonistsListView.itemsSource)[rowB].ColonistName,
                    StringComparison.OrdinalIgnoreCase
                );
        }
        
        // FINISH UP!
        colonistsListView.itemsSource = colonistsVM;
        colonistsListView.RefreshItems();
    }
}
