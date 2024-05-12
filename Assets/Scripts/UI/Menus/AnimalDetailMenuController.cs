using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnimalDetailMenuController : BaseDetailsMenuController
{
    private readonly Dictionary<FieldInfo, Label> fieldLookup = new();
    private VisualElement thoughtsContainer;
    private VisualElement inventoryRow;
    private SmolbeanAnimal targetAnimal;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        target = MapInteractionManager.Instance.Data.SelectedTransform;
        targetAnimal = target.GetComponent<SmolbeanAnimal>();

        Clear();
        DrawMenu();

        InvokeRepeating(nameof(UpdateValues), 1f, 1f);

        targetAnimal.ThoughtsChanged += ThoughtsChanged;
        targetAnimal.Inventory.ContentsChanged += InventoryChanged;
    }

    protected override void OnDisable()
    {
        if (targetAnimal != null)
        {
            targetAnimal.ThoughtsChanged -= ThoughtsChanged;
            targetAnimal.Inventory.ContentsChanged -= InventoryChanged;
        }
        CancelInvoke(nameof(UpdateValues));

        target = null;
        targetAnimal = null;
    }

    protected override void Clear()
    {
        fieldLookup.Clear();
        thoughtsContainer = null;

        base.Clear();
    }

    protected override void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MapInteractionManager.Instance.Data.ForceDeselect();
        MenuController.Instance.CloseAll();
    }

    private void ThoughtsChanged(object sender, EventArgs e)
    {
        UpdateThoughts();
    }

    private void InventoryChanged()
    {
        UpdateInventory();
    }

    private void UpdateLocation()
    {
        var pos = gridManager.GetGameSquareFromWorldCoords(targetAnimal.transform.position);
        var positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
        positionLabel.text = $"{pos.x}λ \u00d7 {pos.y}φ";
    }

    private void DrawMenu()
    {
        var scrollView = document.rootVisualElement.Q<ScrollView>("mainScrollView");
        
        var animalImage = document.rootVisualElement.Q<VisualElement>("animalImage");
        animalImage.style.backgroundImage = targetAnimal.Species.thumbnail;

        var nameLabel = document.rootVisualElement.Q<Label>("nameLabel");
        nameLabel.text = targetAnimal.Stats.name;

        UpdateLocation();

        DisplayFields();

        var colonist = targetAnimal as SmolbeanColonist;
        if (colonist)
            DisplayColonistDetails(colonist);

        Title(scrollView, "𐚱", "Inventory");
        inventoryRow = new VisualElement();
        inventoryRow.AddToClassList("inventoryRow");
        scrollView.Add(inventoryRow);

        UpdateInventory();

        thoughtsContainer = new();
        thoughtsContainer.AddToClassList("thoughtsContainer");
        scrollView.Add(thoughtsContainer);

        UpdateThoughts();
    }

    private void UpdateThoughts()
    {
        thoughtsContainer.Clear();

        int listLength = 4;
        var thoughts = targetAnimal.Thoughts.Reverse().Take(listLength).ToArray();

        for(int i = 0; i < thoughts.Length; i++)
        {
            VisualElement thoughtsRow = new();
            thoughtsRow.AddToClassList(i == 0 ? "thoughtsRowLatest" : "thoughtsRow");
            thoughtsContainer.Add(thoughtsRow);

            string timeStr = DayNightCycleController.Instance.DisplayTime(thoughts[i].timeOfDay);
            string dayStr = DayNightCycleController.Instance.DisplayDay(thoughts[i].day);
            Label todLabel = new($"{dayStr} {timeStr}");
            todLabel.AddToClassList("todLabel");
            thoughtsRow.Add(todLabel);

            Label thoughtLabel = new(thoughts[i].thought);
            thoughtsRow.Add(thoughtLabel);
        }
    }

    private void DisplayFields()
    {
        FieldInfo[] fields = targetAnimal.Stats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields.Where(f => f.Name != "name").OrderBy(f => f.Name))
        {
            VisualElement rowContainer = new();
            rowContainer.AddToClassList("fieldRow");
            document.rootVisualElement.Q<ScrollView>("mainScrollView").Add(rowContainer);

            Label fieldLabel = new(NicifyVariableName(field.Name) + ":");
            fieldLabel.AddToClassList("fieldLabel");
            rowContainer.Add(fieldLabel);

            Label valueLabel = new(GetDisplayValue(targetAnimal, field));
            valueLabel.AddToClassList("valueLabel");
            rowContainer.Add(valueLabel);

            fieldLookup.Add(field, valueLabel);
        }
    }

    private void DisplayColonistDetails(SmolbeanColonist colonist)
    {
        if (colonist.Job == null)
            return;

        var scrollView = document.rootVisualElement.Q<ScrollView>("mainScrollView");

        Title(scrollView, "𐛌", "Job");

        VisualElement jobRow = new();
        jobRow.AddToClassList("jobRow");
        scrollView.Add(jobRow);
        
        Button jobButton = new();
        jobButton.style.backgroundColor = new Color(0, 0, 0, 0);
        jobButton.style.backgroundImage = colonist.Job.JobSpec.thumbnail;
        jobRow.Add(jobButton);

        Label jobLabel = new(colonist.Job.JobSpec.jobName);
        jobRow.Add(jobLabel);
    }

    private void UpdateInventory()
    {
        inventoryRow.Clear();

        if(targetAnimal.Inventory.IsEmpty())
        {
            Label emptyLabel = new("Nothing");
            inventoryRow.Add(emptyLabel);
            return;
        }

        foreach (var item in targetAnimal.Inventory.Totals)
        {
            Button button = new();
            button.text = item.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = item.dropSpec.thumbnail;
            inventoryRow.Add(button);
        }
    }

    private void UpdateValues()
    {
        var animal = target.GetComponent<SmolbeanAnimal>();

        foreach (var (field, label) in fieldLookup)
        {
            label.text = GetDisplayValue(animal, field);
        }

        UpdateLocation();
    }

    private string NicifyVariableName(string name)
    {
        StringBuilder result = new();

        foreach(char c in name)
        {
            if (char.IsUpper(c))
                result.Append(" " + c);
            else
                result.Append(c);
        }

        result[0] = char.ToUpper(result[0]);

        return result.ToString();
    }

    private static string GetDisplayValue(SmolbeanAnimal animal, FieldInfo field)
    {
        var value = field.GetValue(animal.Stats);

        if (field.FieldType == typeof(float))
        {
            if (field.Name.ToLower() == "age")
                return DayNightCycleController.Instance.DurationToString((float)value);
            else
                return string.Format("{0:0.0}", value);
        }

        return value.ToString();
    }
}
