using System;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class AnimalDetailMenuController : SmolbeanMenu
{
    protected UIDocument document;
    private VisualElement root;
    protected GridManager gridManager;
    protected SoundPlayer soundPlayer;
    protected Transform target;

    private SmolbeanAnimal targetAnimal;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        gridManager = FindFirstObjectByType<GridManager>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        document.rootVisualElement.Q<Button>("closeButton").clicked += CloseButtonClicked;
        
        target = MapInteractionManager.Instance.Data.SelectedTransform;
        targetAnimal = target.GetComponent<SmolbeanAnimal>();

        UpdateAll();

        InvokeRepeating(nameof(UpdateAll), 1f, 1f);

        targetAnimal.ThoughtsChanged += ThoughtsChanged;
        targetAnimal.Inventory.ContentsChanged += UpdateInventory;
    }

    private void OnDisable()
    {
        if (targetAnimal != null)
        {
            targetAnimal.ThoughtsChanged -= ThoughtsChanged;
            targetAnimal.Inventory.ContentsChanged -= UpdateInventory;
        }
        CancelInvoke(nameof(UpdateAll));

        target = null;
        targetAnimal = null;
    }

    private void UpdateAll()
    {
        if (targetAnimal == null || targetAnimal.transform == null)
            return; // Try to avoid null refs!

        // Basic values
        root.Q<VisualElement>("thumbnail").style.backgroundImage = targetAnimal.Species.thumbnail;
        root.Q<Label>("nameLabel").text = targetAnimal.Stats.name;
        root.Q<ProgressBar>("healthBar").value = targetAnimal.Stats.health;
        root.Q<ProgressBar>("foodBar").value = targetAnimal.Stats.foodLevel;
        root.Q<Label>("ageLabel").text = DayNightCycleController.Instance.DurationToString(targetAnimal.Stats.age);
        var pos = gridManager.GetGameSquareFromWorldCoords(targetAnimal.transform.position);
        root.Q<Label>("positionLabel").text = $"{pos.x}λ x {pos.y}φ";
        root.Q<Label>("speedLabel").text = $"{targetAnimal.Stats.speed:0.0}";

        // Buffs
        var buffsList = root.Q<ListView>("buffsListView");
        buffsList.itemsSource = targetAnimal.Buffs;
        buffsList.makeItem = () => new Label();
        buffsList.bindItem = (element, i) =>
        {
            var label = (Label)element;
            label.text = $"{targetAnimal.Buffs[i].Spec.symbol} {targetAnimal.Buffs[i].Spec.buffName}";
            element.tooltip = targetAnimal.Buffs[i].Spec.description;
        };
        buffsList.RefreshItems();

        UpdateJob();
        UpdateInventory();
        UpdateThoughts();
    }

    private void UpdateJob()
    {
        var colonist = targetAnimal as SmolbeanColonist;
        if (colonist)
        {
            root.Q<VisualElement>("jobContainer").visible = colonist.Job != null;
            if (colonist.Job != null)
            {
                root.Q<VisualElement>("jobThumb").style.backgroundImage = colonist.Job.JobSpec.thumbnail;
                root.Q<Label>("jobLabel").text = colonist.Job.JobSpec.jobName;
            }
        }
        else
        {
            root.Q<VisualElement>("jobContainer").visible = false;
        }
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MapInteractionManager.Instance.Data.ForceDeselect();
        MenuController.Instance.CloseAll();
    }

    private void ThoughtsChanged(object sender, EventArgs e)
    {
        UpdateThoughts();
    }

    private void UpdateThoughts()
    {
        var container = root.Q<VisualElement>("thoughtsContainer");
        container.Clear();

        int listLength = 20;
        var thoughts = targetAnimal.Thoughts.Reverse().Take(listLength).ToArray();

        for(int i = 0; i < thoughts.Length; i++)
        {
            VisualElement thoughtsRow = new();
            thoughtsRow.AddToClassList(i == 0 ? "thoughtsRowLatest" : "thoughtsRow");
            container.Add(thoughtsRow);

            string timeStr = DayNightCycleController.Instance.DisplayTime(thoughts[i].timeOfDay);
            string dayStr = DayNightCycleController.Instance.DisplayDay(thoughts[i].day);
            Label todLabel = new($"{dayStr} {timeStr}");
            todLabel.AddToClassList("todLabel");
            thoughtsRow.Add(todLabel);

            Label thoughtLabel = new(thoughts[i].thought);
            thoughtsRow.Add(thoughtLabel);
        }
    }

    private void UpdateInventory()
    {
        var container = root.Q<VisualElement>("inventoryContainer");
        container.Clear();

        if(targetAnimal.Inventory.IsEmpty())
        {
            Label emptyLabel = new("Empty");
            container.Add(emptyLabel);
            return;
        }

        foreach (var item in targetAnimal.Inventory.Totals)
        {
            Button button = new();
            button.text = item.quantity.ToString();
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = item.dropSpec.thumbnail;
            container.Add(button);
        }
    }
}
