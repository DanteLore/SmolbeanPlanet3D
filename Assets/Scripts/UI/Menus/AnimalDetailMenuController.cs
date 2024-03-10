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

    protected override void Refresh()
    {
        if (ReferenceEquals(animalController.TargetTransform, target))
        {
            if (target != null)
                UpdateValues();
        }
        else
        {
            target = animalController.TargetTransform;

            Clear();

            if (target != null)
                DrawMenu();
        }
    }

    private void UpdateValues()
    {
        var animal = target.GetComponent<SmolbeanAnimal>();

        foreach (var (field, label) in fieldLookup)
        {
            label.text = field.Name + ": " + field.GetValue(animal.Stats);
        }
    }

    private void DrawMenu()
    {
        var animal = target.GetComponent<SmolbeanAnimal>();

        var animalImage = document.rootVisualElement.Q<VisualElement>("animalImage");
        animalImage.style.backgroundImage = animal.species.thumbnail;

        var mainScrollView = document.rootVisualElement.Q<ScrollView>("mainScrollView");
        FieldInfo[] fields = animal.Stats.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            Label fieldLabel = new();
            mainScrollView.Add(fieldLabel);
            fieldLabel.text = field.Name + ": " + field.GetValue(animal.Stats);

            fieldLookup.Add(field, fieldLabel);
        }
    }
}
