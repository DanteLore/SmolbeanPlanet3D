using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class JobsMenuController : SmolbeanMenu
{
    private UIDocument document;

    VisualElement listContainer;
    private Label colonistCountLabel;
    private Label jobCountLabel;
    SoundPlayer soundPlayer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        listContainer = document.rootVisualElement.Q<VisualElement>("listContainer");

        colonistCountLabel = document.rootVisualElement.Q<Label>("colonistCountLabel");
        jobCountLabel = document.rootVisualElement.Q<Label>("jobCountLabel");

        InvokeRepeating(nameof(RefreshJobsList), 0f, 1.0f);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(RefreshJobsList));
    }

    private void RefreshJobsList()
    {
        int colonistCount = AnimalController.Instance.GetAnimalsByType<SmolbeanColonist>().Count();
        int jobCount = JobController.Instance.AllJobs.Count();

        colonistCountLabel.text = $"Colonists: {colonistCount}";
        jobCountLabel.text = $"Jobs: {jobCount}";

        listContainer.Clear();

        if (JobController.Instance.AllJobs.Any())
        {
            foreach (Job job in JobController.Instance.AllJobs)
                AddRow(job);
        }
    }
    
    private void AddRow(Job job)
    {
        var row = new VisualElement();
        row.AddToClassList("jobRow");
        listContainer.Add(row);

        Toggle jobEnabledToggle = new();
        row.Add(jobEnabledToggle);
        jobEnabledToggle.value = job.IsOpen;
        jobEnabledToggle.RegisterValueChangedCallback(v =>
        {
            if (v.newValue)
                job.Open();
            else
                job.Terminate();
        });

        var jobIcon = new Button();
        jobIcon.AddToClassList("icon");
        jobIcon.style.backgroundColor = new Color(0, 0, 0, 0);
        jobIcon.style.backgroundImage = job.JobSpec.thumbnail;
        row.Add(jobIcon);

        row.Add(new Label { text = job.JobSpec.jobName });

        var buildingIcon = new Button();
        buildingIcon.AddToClassList("icon");
        buildingIcon.style.backgroundColor = new Color(0, 0, 0, 0);
        buildingIcon.style.backgroundImage = job.Building.BuildingSpec.thumbnail;
        row.Add(buildingIcon);

        row.Add(new Label { text = job.Building.name });

        if (job.Colonist != null)
        {
            var colonistIcon = new Button();
            colonistIcon.AddToClassList("icon");
            colonistIcon.style.backgroundColor = new Color(0, 0, 0, 0);
            colonistIcon.style.backgroundImage = job.Colonist.species.thumbnail;
            row.Add(colonistIcon);

            row.Add(new Label { text = job.Colonist.Stats.name });
        }
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }
}
