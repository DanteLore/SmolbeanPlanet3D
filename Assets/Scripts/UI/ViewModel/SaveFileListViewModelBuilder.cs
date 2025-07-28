
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public class SaveFileListViewModelBuilder
{
    public static void BuildFileView(MultiColumnListView listView, List<SaveFileViewModel> saveFiles)
    {
        // FILENAME
        var nameColumn = listView.columns.FirstOrDefault(c => c.name == "nameColumn");
        if (nameColumn != null)
        {
            nameColumn.makeCell = () => new Label();
            nameColumn.bindCell = (cell, i) =>
            {
                var items = (List<SaveFileViewModel>)listView.itemsSource;
                cell.Q<Label>().text = items[i].Name;
            };
        }
        nameColumn.comparison = (rowA, rowB) =>
                ((List<SaveFileViewModel>)listView.itemsSource)[rowA].Name.CompareTo(
                    ((List<SaveFileViewModel>)listView.itemsSource)[rowB].Name);

        // MODIFIED TIME
        var timeColumn = listView.columns.FirstOrDefault(c => c.name == "timeColumn");
        if (timeColumn != null)
        {
            timeColumn.makeCell = () => new Label();
            timeColumn.bindCell = (cell, i) =>
            {
                var items = (List<SaveFileViewModel>)listView.itemsSource;
                cell.Q<Label>().text = items[i].ModifiedTimeString;
            };
        }
        timeColumn.comparison = (rowA, rowB) =>
                ((List<SaveFileViewModel>)listView.itemsSource)[rowA].ModifiedTime.CompareTo(
                    ((List<SaveFileViewModel>)listView.itemsSource)[rowB].ModifiedTime);

        listView.itemsSource = saveFiles;
        listView.RefreshItems();
    }
}
