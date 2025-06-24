using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Scripting;
using System;

public class OfferingViewBuilder
{
    public static void BuildOfferingsView(MultiColumnListView listView, List<Offering> offerings, SoundPlayer soundPlayer)
    {
        // STATUS
        var statusColumn = listView.columns.FirstOrDefault(c => c.name == "statusColumn");
        if (statusColumn != null)
        {
            statusColumn.makeCell = () =>
            {
                var btn = new Button();
                btn.AddToClassList("accept-button");
                return btn;
            };
            statusColumn.bindCell = (cell, i) =>
            {
                var items = (List<OfferingViewModel>)listView.itemsSource;
                var button = cell.Q<Button>();
                button.SetEnabled(items[i].ShowStartButton);
                button.text = items[i].ShowStartButton ? "Accept" : "Accepted";

                if (button.userData is Action oldCb)
                    button.clicked -= oldCb;

                Action callback = () =>
                {
                    soundPlayer.Play("Magic2");
                    items[i].StartOffering();
                };
                button.clicked += callback;
                button.userData = callback;
            };
            statusColumn.comparison = (rowA, rowB) =>
                    ((List<OfferingViewModel>)listView.itemsSource)[rowA].ShowStartButton.CompareTo(
                        ((List<OfferingViewModel>)listView.itemsSource)[rowB].ShowStartButton);
        }

        // DURATION
        var durationColumn = listView.columns.FirstOrDefault(c => c.name == "durationColumn");
        if (durationColumn != null)
        {
            durationColumn.makeCell = () => new ProgressBar();
            durationColumn.bindCell = (cell, i) =>
            {
                var items = (List<OfferingViewModel>)listView.itemsSource;
                var pb = cell.Q<ProgressBar>();
                float percent = items[i].RemainingTime / items[i].InitialDuration;

                pb.highValue = items[i].InitialDuration;
                pb.value = items[i].RemainingTime;
                pb.title = items[i].RemainingTimeString;
                if (percent < 0.2f)
                {
                    pb.AddToClassList("time-bar-danger");
                    pb.RemoveFromClassList("time-bar-warning");
                    pb.RemoveFromClassList("time-bar-good");
                }
                else if (percent < 0.4f)
                {
                    pb.AddToClassList("time-bar-warning");
                    pb.RemoveFromClassList("time-bar-good");
                    pb.RemoveFromClassList("time-bar-danger");
                }
                else
                {
                    pb.AddToClassList("time-bar-good");
                    pb.RemoveFromClassList("time-bar-danger");
                    pb.RemoveFromClassList("time-bar-warning");
                }
            };
            durationColumn.comparison = (rowA, rowB) =>
                    ((List<OfferingViewModel>)listView.itemsSource)[rowA].RemainingTime.CompareTo(
                        ((List<OfferingViewModel>)listView.itemsSource)[rowB].RemainingTime);
        }

        // REWARD
        var rewardColumn = listView.columns.FirstOrDefault(c => c.name == "rewardColumn");
        if (rewardColumn != null)
        {
            rewardColumn.makeCell = () =>
            {
                var label = new Label("manaLabel");
                label.AddToClassList("mana-label");
                return label;
            };
            rewardColumn.bindCell = (cell, i) =>
            {
                var items = (List<OfferingViewModel>)listView.itemsSource;
                cell.Q<Label>().text = items[i].RewardString;
            };
            rewardColumn.comparison = (rowA, rowB) =>
                    ((List<OfferingViewModel>)listView.itemsSource)[rowA].Reward.CompareTo(
                        ((List<OfferingViewModel>)listView.itemsSource)[rowB].Reward);
        }

        // ITEMS
        var itemsColumn = listView.columns.FirstOrDefault(c => c.name == "itemsColumn");
        if (itemsColumn != null)
        {
            itemsColumn.makeCell = () =>
            {
                var ve = new VisualElement();
                ve.AddToClassList("items-list");
                return ve;
            };
            itemsColumn.bindCell = (cell, i) =>
            {
                var items = (List<OfferingViewModel>)listView.itemsSource;
                cell.Clear(); // Need to clear the cell as we're regenerating content on re-bind here
                foreach (var oi in items[i].Items)
                {
                    var row = MakeThumbAndLabelCell();
                    row.Q<Label>("valueLabel").text = oi.DisplayLabel;
                    row.Q<Image>("offeringThumbnail").image = oi.Thumbnail;

                    cell.Add(row);
                }
            };
        }

        var requestsVM = offerings.Select(o => new OfferingViewModel(o)).ToList();
        listView.itemsSource = requestsVM;
        listView.RefreshItems();
    }

    private static VisualElement MakeThumbAndLabelCell()
    {
        var container = new VisualElement();
        container.AddToClassList("offering-thumb-and-label-row");

        var icon = new Image() { name = "offeringThumbnail" };
        icon.AddToClassList("offering-thumb-image");

        container.Add(icon);

        var lbl = new Label { name = "valueLabel" };
        lbl.AddToClassList("offering-table-label");
        container.Add(lbl);

        return container;
    }
}
