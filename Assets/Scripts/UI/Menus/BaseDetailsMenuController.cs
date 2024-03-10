using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseDetailsMenuController : SmolbeanMenu
{
    protected UIDocument document;
    protected GridManager gridManager;
    protected SoundPlayer soundPlayer;
    protected Transform target;

    protected virtual void OnEnable()
    {
        document = GetComponent<UIDocument>();
        gridManager = FindFirstObjectByType<GridManager>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

    }

    void Update()
    {
        Refresh();
    }

    protected abstract void CloseButtonClicked();

    protected abstract void Refresh();

    protected virtual void Clear()
    {
        document.rootVisualElement.Q<ScrollView>("mainScrollView").Clear();
    }
}
