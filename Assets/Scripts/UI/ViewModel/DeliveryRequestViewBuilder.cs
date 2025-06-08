using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class DeliveryRequestViewBuilder
{
    public static void BuildDeliveryRequestView(MultiColumnListView listView, List<DeliveryRequest> requests)
    {
        var priorities = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        // Priority
        var priorityColumn = listView.columns.FirstOrDefault(c => c.name == "priorityColumn");
        if (priorityColumn != null)
        {
            priorityColumn.makeCell = () =>
            {
                return new DropdownField { choices = priorities };
            };
            priorityColumn.bindCell = (cell, i) =>
            {
                var dropdown = cell.Q<DropdownField>();

                if (dropdown.userData is EventCallback<ChangeEvent<string>> oldCb)
                    dropdown.UnregisterValueChangedCallback(oldCb);

                var items = (List<DeliveryRequestViewModel>)listView.itemsSource;
                var item = items[i];
                dropdown.value = item.Priority.ToString();

                EventCallback<ChangeEvent<string>> newCb = evt =>
                {
                    var dd = (DropdownField)evt.currentTarget;
                    item.Priority = int.Parse(evt.newValue);
                };

                dropdown.RegisterValueChangedCallback(newCb);
                dropdown.userData = newCb;
            };
            priorityColumn.comparison = (rowA, rowB) =>
            {
                return
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowA].Priority.CompareTo(
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowB].Priority);
            };
        }

        // ITEM
        var itemColumn = listView.columns.FirstOrDefault(c => c.name == "itemColumn");
        if (itemColumn != null)
        {
            itemColumn.makeCell = MakeThumbAndLabelCell;
            itemColumn.bindCell = (cell, i) =>
            {
                var items = (List<DeliveryRequestViewModel>)listView.itemsSource;
                var m = items[i];
                cell.Q<Label>("valueLabel").text = items[i].ItemText;
                cell.Q<Image>("drThumbnail").image = items[i].ItemThumbnail;
            };
            itemColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowA].ItemName,
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowB].ItemName,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        // COLONIST
        var colonistColumn = listView.columns.FirstOrDefault(c => c.name == "colonistColumn");
        if (colonistColumn != null)
        {
            colonistColumn.makeCell = MakeThumbAndLabelCell;
            colonistColumn.bindCell = (cell, i) =>
            {
                var items = (List<DeliveryRequestViewModel>)listView.itemsSource;
                var m = items[i];
                cell.Q<Label>("valueLabel").text = items[i].ColonistName;
                cell.Q<Image>("drThumbnail").image = items[i].ColonistThumbnail;
            };
            colonistColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowA].ColonistName,
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowB].ColonistName,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        // BUILDING
        var buildingColumn = listView.columns.FirstOrDefault(c => c.name == "buildingColumn");
        if (buildingColumn != null)
        {
            buildingColumn.makeCell = MakeThumbAndLabelCell;
            buildingColumn.bindCell = (cell, i) =>
            {
                var items = (List<DeliveryRequestViewModel>)listView.itemsSource;
                var m = items[i];
                cell.Q<Label>("valueLabel").text = items[i].BuildingName;
                cell.Q<Image>("drThumbnail").image = items[i].BuildingThumbnail;
            };
            buildingColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowA].BuildingName,
                    ((List<DeliveryRequestViewModel>)listView.itemsSource)[rowB].BuildingName,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        var requestsVM = requests.Select(r => new DeliveryRequestViewModel(r)).ToList();
        listView.itemsSource = requestsVM;
        listView.RefreshItems();
    }

    private static VisualElement MakeThumbAndLabelCell()
    {
        var container = new VisualElement();
        container.AddToClassList("dr-thumb-and-label-row");

        var icon = new Image() { name = "drThumbnail" };
        icon.AddToClassList("dr-thumb-image");
        container.Add(icon);

        var lbl = new Label { name = "valueLabel" };
        lbl.AddToClassList("dr-table-label");
        container.Add(lbl);

        return container;
    }
}
