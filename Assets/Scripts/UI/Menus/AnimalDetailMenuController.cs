using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using System.Linq;
using System.Text;
using UnityEngine;

public class AnimalDetailMenuController : BaseDetailsMenuController
{
    private AnimalDetailController animalDetailController;
    private readonly Dictionary<FieldInfo, Label> fieldLookup = new();
    private VisualElement thoughtsContainer;

    protected override void OnEnable()
    {
        animalDetailController = FindFirstObjectByType<AnimalDetailController>();
        target = null;

        base.OnEnable();
    }

    protected void OnDisable()
    {
        if (target != null)
            target.GetComponent<SmolbeanAnimal>().ThoughtsChanged -= ThoughtsChanged;

        target = null;

        animalDetailController.ClearSelection();
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
        MenuController.Instance.CloseAll();
    }

    protected override void Update()
    {
        // Already showing this animal, just update the fields
        if (ReferenceEquals(animalDetailController.TargetTransform, target) && target != null)
        {
            UpdateValues();
            return;
        }

        // Target transform cleared.  Close.
        if (animalDetailController.TargetTransform == null)
        {
            MenuController.Instance.CloseAll();
            return;
        }

        // Must be a new target, hook up
        // But first unhook any existing one...
        if (target != null)
            target.GetComponent<SmolbeanAnimal>().ThoughtsChanged -= ThoughtsChanged;

        target = animalDetailController.TargetTransform;
        target.GetComponent<SmolbeanAnimal>().ThoughtsChanged += ThoughtsChanged;

        Clear();
        DrawMenu();
    }

    private void ThoughtsChanged(object sender, EventArgs e)
    {
        UpdateThoughts(target.GetComponent<SmolbeanAnimal>());
    }

    private void UpdateLocation(SmolbeanAnimal animal)
    {
        var pos = gridManager.GetGameSquareFromWorldCoords(animal.transform.position);
        var positionLabel = document.rootVisualElement.Q<Label>("positionLabel");
        positionLabel.text = $"{pos.x}λ \u00d7 {pos.y}φ";
    }

    private void DrawMenu()
    {
        var animal = target.GetComponent<SmolbeanAnimal>();

        var animalImage = document.rootVisualElement.Q<VisualElement>("animalImage");
        animalImage.style.backgroundImage = animal.species.thumbnail;

        var nameLabel = document.rootVisualElement.Q<Label>("nameLabel");
        nameLabel.text = animal.Stats.name;

        UpdateLocation(animal);

        DisplayFields(animal);

        thoughtsContainer = new();
        thoughtsContainer.AddToClassList("thoughtsContainer");
        document.rootVisualElement.Q<ScrollView>("mainScrollView").Add(thoughtsContainer);

        UpdateThoughts(animal);
    }

    private void UpdateThoughts(SmolbeanAnimal animal)
    {
        thoughtsContainer.Clear();

        int listLength = 4;
        var thoughts = animal.Thoughts.Reverse().Take(listLength).ToArray();

        for(int i = 0; i < thoughts.Length; i++)
        {
            VisualElement thoughtsRow = new();
            thoughtsRow.AddToClassList(i == 0 ? "thoughtsRowLatest" : "thoughtsRow");
            thoughtsContainer.Add(thoughtsRow);

            string timeStr = DayNightCycleController.Instance.DisplayTime(thoughts[i].timeOfDay);
            string dayStr = DayNightCycleController.Instance.DisplayDay(thoughts[i].day);
            Label todLabel = new();
            todLabel.text = $"{dayStr} {timeStr}";
            todLabel.AddToClassList("todLabel");
            thoughtsRow.Add(todLabel);

            Label thoughtLabel = new();
            thoughtLabel.text = thoughts[i].thought;
            thoughtsRow.Add(thoughtLabel);
        }
    }

    private void DisplayFields(SmolbeanAnimal animal)
    {
        FieldInfo[] fields = animal.Stats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields.Where(f => f.Name != "name").OrderBy(f => f.Name))
        {
            VisualElement rowContainer = new();
            rowContainer.AddToClassList("fieldRow");
            document.rootVisualElement.Q<ScrollView>("mainScrollView").Add(rowContainer);

            Label fieldLabel = new();
            fieldLabel.AddToClassList("fieldLabel");
            fieldLabel.text = NicifyVariableName(field.Name) + ":";
            rowContainer.Add(fieldLabel);

            Label valueLabel = new();
            valueLabel.AddToClassList("valueLabel");
            valueLabel.text = GetDisplayValue(animal, field);
            rowContainer.Add(valueLabel);

            fieldLookup.Add(field, valueLabel);
        }
    }

    private void UpdateValues()
    {
        var animal = target.GetComponent<SmolbeanAnimal>();

        foreach (var (field, label) in fieldLookup)
        {
            label.text = GetDisplayValue(animal, field);
        }

        UpdateLocation(animal);
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
