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

    void OnDisable()
    {
        target = null;
    }

    protected virtual void Update() { }

    protected abstract void CloseButtonClicked();

    protected virtual void Clear()
    {
        document.rootVisualElement.Q<ScrollView>("mainScrollView").Clear();
    }

    protected static void Title(VisualElement parent, string symbol, string text, string symbolClass = "notoLinearA")
    {
        var titleContainer = new VisualElement();
        titleContainer.AddToClassList("titleRow");
        parent.Add(titleContainer);

        Label symbolLabel = new();
        symbolLabel.AddToClassList(symbolClass);
        symbolLabel.AddToClassList("bigLabel");
        titleContainer.Add(symbolLabel);
        symbolLabel.text = symbol;

        Label textLabel = new();
        titleContainer.Add(textLabel);
        textLabel.AddToClassList("titleLabel");
        textLabel.text = text;
    }
}
