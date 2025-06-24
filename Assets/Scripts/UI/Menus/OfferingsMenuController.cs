using UnityEngine;
using UnityEngine.UIElements;

public class OfferingsMenuController : SmolbeanMenu
{
    private UIDocument document;
    private MultiColumnListView listView;
    private SoundPlayer soundPlayer;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        listView = document.rootVisualElement.Q<MultiColumnListView>("offeringsListView");

        InvokeRepeating(nameof(RedrawList), 1.0f, 1.0f);
        RedrawList();
    }

    private void RedrawList()
    {
        // soon
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }
}
