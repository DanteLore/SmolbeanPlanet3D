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

        InvokeRepeating(nameof(UpdateJobs), 1.0f, 1.0f);
        UpdateJobs();
    }

    void OnDisable()
    {
        CancelInvoke(nameof(UpdateJobs));
    }

    private void UpdateJobs()
    {
        var jobs = JobController.Instance.AllJobs.ToArray();

        var jobsListView = document.rootVisualElement.Q<MultiColumnListView>("jobsListView");
        JobViewBuilder.BuildJobView(jobsListView, jobs);

        int colonistCount = AnimalController.Instance.GetAnimalsByType<SmolbeanColonist>().Count();
        int jobCount = JobController.Instance.AllJobs.Count();

        colonistCountLabel.text = $"Colonists: {colonistCount}";
        jobCountLabel.text = $"Jobs: {jobCount}";
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }
}
