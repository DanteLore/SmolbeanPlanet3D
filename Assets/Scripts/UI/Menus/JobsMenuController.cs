using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class JobsMenuController : SmolbeanMenu
{
    private UIDocument document;

    VisualElement listContainer;
    SoundPlayer soundPlayer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;
        
        listContainer = document.rootVisualElement.Q<VisualElement>("listContainer");

        InvokeRepeating(nameof(RefreshJobsList), 0f, 1.0f);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(RefreshJobsList));
    }

    private void RefreshJobsList()
    {
        listContainer.Clear();

        if (JobController.Instance.Vacancies.Any())
        {
            listContainer.Add(new Label { text = "Vacancies:" });

            foreach (Job job in JobController.Instance.Vacancies)
                AddRow(job);
        }

        if (JobController.Instance.AssignedJobs.Any())
        {
            listContainer.Add(new Label { text = "Assigned Jobs:" });

            foreach (Job job in JobController.Instance.AssignedJobs)
                AddRow(job);
        }
    }
    
    private void AddRow(Job job)
    {
        var row = new VisualElement();
        row.AddToClassList("jobRow");
        listContainer.Add(row);

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
