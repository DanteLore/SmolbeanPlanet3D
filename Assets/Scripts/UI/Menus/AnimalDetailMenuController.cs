using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class AnimalDetailMenuController : BaseDetailsMenuController
{
    private AnimalController animalController;
    private Dictionary<FieldInfo, Label> fieldLookup = new();

    protected override void OnEnable()
    {
        animalController = FindFirstObjectByType<AnimalController>();

        base.OnEnable();
    }

    protected override void Clear()
    {
        fieldLookup.Clear();
        base.Clear();
    }

    protected override void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        animalController.ClearSelection();
        MenuController.Instance.CloseAll();
    }

    protected override void Update()
    {
        if(animalController.TargetTransform == null)
        {
            target = null;
            Clear();
        }
        else if (ReferenceEquals(animalController.TargetTransform, target))
        {
            UpdateValues();
        }
        else
        {
            target = animalController.TargetTransform;
            Clear();
            DrawMenu();
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

        var mainScrollView = document.rootVisualElement.Q<ScrollView>("mainScrollView");
        FieldInfo[] fields = animal.Stats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields.Where(f => f.Name != "name"))
        {
            VisualElement rowContainer = new();
            rowContainer.AddToClassList("fieldRow");
            mainScrollView.Add(rowContainer);

            Label fieldLabel = new();
            fieldLabel.AddToClassList("fieldLabel");
            fieldLabel.text = field.Name + ":";
            rowContainer.Add(fieldLabel);

            Label valueLabel = new();
            valueLabel.AddToClassList("valueLabel");
            valueLabel.text = GetDisplayValue(animal, field);
            rowContainer.Add(valueLabel);

            fieldLookup.Add(field, valueLabel);
        }
    }

    private static string GetDisplayValue(SmolbeanAnimal animal, FieldInfo field)
    {
        var value = field.GetValue(animal.Stats);

        if (field.FieldType == typeof(float))
            return String.Format("{0:0.0}", value);

        return value.ToString();
    }
}
