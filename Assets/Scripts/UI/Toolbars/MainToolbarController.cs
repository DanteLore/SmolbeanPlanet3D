using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class MainToolbarController : MonoBehaviour
{
    UIDocument doc;
    private Button offeringButton;
    private SoundPlayer soundPlayer;

    private void Start()
    {
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
    }

    private void OnEnable()
    {
        doc = GetComponent<UIDocument>();

        doc.rootVisualElement.Q<Button>("buildToolbarButton").clicked += () => ToolbarController.Instance.ShowToolbar("BuildToolbar");
        doc.rootVisualElement.Q<Button>("mapButton").clicked += () => MenuController.Instance.ShowMenu("MapMenu");
        doc.rootVisualElement.Q<Button>("inventoryButton").clicked += () => MenuController.Instance.ShowMenu("InventoryMenu");
        doc.rootVisualElement.Q<Button>("jobsButton").clicked += () => MenuController.Instance.ShowMenu("JobsMenu");
        doc.rootVisualElement.Q<Button>("deliveryRequestsButton").clicked += () => MenuController.Instance.ShowMenu("DeliveryRequestsMenu");
        doc.rootVisualElement.Q<Button>("graphButton").clicked += () => MenuController.Instance.ShowMenu("GraphMenu");
        offeringButton = doc.rootVisualElement.Q<Button>("offeringsButton");
        offeringButton.clicked += () => MenuController.Instance.ShowMenu("OfferingsMenu");

        OfferingController.Instance.OnOfferingCreated += OfferingCreated;
    }

    private void OnDisable()
    {
        OfferingController.Instance.OnOfferingCreated -= OfferingCreated;
    }

    private void OfferingCreated(Offering offering)
    {
        StartCoroutine(FlashOfferingButton());
    }

    private IEnumerator FlashOfferingButton()
    {
        soundPlayer.Play("Magic2");
        offeringButton.AddToClassList("glow");

        yield return new WaitForSeconds(2f);

        offeringButton.RemoveFromClassList("glow");
        offeringButton.AddToClassList("glow-out");

        yield return new WaitForSeconds(4f);

        offeringButton.RemoveFromClassList("glow-out");
    }
}
