using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DeliveryRequestsMenu : SmolbeanMenu
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

        listView = document.rootVisualElement.Q<MultiColumnListView>("requestsListView");

        InvokeRepeating(nameof(RedrawList), 1.0f, 1.0f);
        RedrawList();
    }

    void OnDisable()
    {
        CancelInvoke(nameof(RedrawList));
    }

    private void RedrawList()
    {
        listView.Clear();

        var allRequests = DeliveryManager.Instance.ClaimedDeliveryRequests.ToList().Union(DeliveryManager.Instance.UnclaimedDeliveryRequests).ToList();

        DeliveryRequestViewBuilder.BuildDeliveryRequestView(listView, allRequests);
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }
}
