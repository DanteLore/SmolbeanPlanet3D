using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public static class JobViewBuilder
{
    public static void BuildJobView(MultiColumnListView jobsListView, Job[] jobs)
    {
        var jobsVM = jobs.Select(j => new JobViewModel(j)).ToList();

        // BUILDING
        var buildingColumn = jobsListView.columns.FirstOrDefault(c => c.name == "buildingColumn");
        if (buildingColumn != null)
        {
            buildingColumn.makeCell = MakeThumbAndLabelCell;
            buildingColumn.bindCell = (cell, i) =>
            {
                var jobsList = (List<JobViewModel>)jobsListView.itemsSource;
                var m = jobsList[i];
                cell.Q<Label>("valueLabel").text = jobsList[i].BuildingName;
                cell.Q<Image>("jobRowThumbnail").image = jobsList[i].BuildingThumbnail;
            };
            buildingColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<JobViewModel>)jobsListView.itemsSource)[rowA].BuildingName,
                    ((List<JobViewModel>)jobsListView.itemsSource)[rowB].BuildingName,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        // JOB TITLE
        var jobTitleColumn = jobsListView.columns.FirstOrDefault(c => c.name == "jobTitleColumn");
        if (jobTitleColumn != null)
        {
            jobTitleColumn.makeCell = MakeThumbAndLabelCell;
            jobTitleColumn.bindCell = (cell, i) =>
            {
                var jobsList = (List<JobViewModel>)jobsListView.itemsSource;
                var m = jobsList[i];
                cell.Q<Label>("valueLabel").text = jobsList[i].JobTitle;
                cell.Q<Image>("jobRowThumbnail").image = jobsList[i].JobThumbnail;
            };
            jobTitleColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<JobViewModel>)jobsListView.itemsSource)[rowA].JobTitle,
                    ((List<JobViewModel>)jobsListView.itemsSource)[rowB].JobTitle,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        // COLONIST
        var colonistColumn = jobsListView.columns.FirstOrDefault(c => c.name == "colonistNameColumn");
        if (colonistColumn != null)
        {
            colonistColumn.makeCell = MakeThumbAndLabelCell;
            colonistColumn.bindCell = (cell, i) =>
            {
                var jobsList = (List<JobViewModel>)jobsListView.itemsSource;
                var m = jobsList[i];
                cell.Q<Label>("valueLabel").text = jobsList[i].ColonistName;
                cell.Q<Image>("jobRowThumbnail").image = jobsList[i].ColonistThumbnail;
            };
            colonistColumn.comparison = (rowA, rowB) =>
                string.Compare(
                    ((List<JobViewModel>)jobsListView.itemsSource)[rowA].ColonistName,
                    ((List<JobViewModel>)jobsListView.itemsSource)[rowB].ColonistName,
                    StringComparison.OrdinalIgnoreCase
                );
        }

        // ENABLED
        var enabledColumn = jobsListView.columns.FirstOrDefault(c => c.name == "enabledColumn");
        if (enabledColumn != null)
        {
            enabledColumn.makeCell = () => new Toggle { userData = null };

            enabledColumn.bindCell = (cell, i) =>
            {
                var toggle = (Toggle)cell;
                var job = ((List<JobViewModel>)jobsListView.itemsSource)[i];

                // If we previously registered a callback, remove it now
                if (toggle.userData is EventCallback<ChangeEvent<bool>> oldCallback)
                    toggle.UnregisterValueChangedCallback(oldCallback);

                toggle.value = job.Enabled;

                EventCallback<ChangeEvent<bool>> newCallback = evt =>
                {
                    job.Enabled = evt.newValue;
                    jobsListView.RefreshItem(i);
                };

                toggle.RegisterValueChangedCallback(newCallback);
                // Store so we can unregister next time bindCell fires
                toggle.userData = newCallback;
            };
        }

        // FINISH UP!
        jobsListView.itemsSource = jobsVM;
        jobsListView.RefreshItems();
    }

    private static VisualElement MakeThumbAndLabelCell()
    {
        var container = new VisualElement();
        container.AddToClassList("jobs-thumb-and-label-row");

        var icon = new Image() { name = "jobRowThumbnail" };
        icon.AddToClassList("jobs-thumb-image");
        container.Add(icon);

        var lbl = new Label { name = "valueLabel" };
        lbl.AddToClassList("jobs-table-label");
        container.Add(lbl);

        return container;
    }
}
