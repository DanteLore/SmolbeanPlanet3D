using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildToolbarController : MonoBehaviour
{
    private SoundPlayer soundPlayer;
    private VisualElement root;
    private ScrollView buttonScroller;
    private VisualElement tooltipContainer;
    private Button scrollLeftButton;
    private Button scrollRightButton;
    private const float scrollStep = 110f;

    private void Start()
    {
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
    }

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        var mainMenuButton = root.Q<Button>("mainToolbarButton");
        mainMenuButton.clicked += MainMenuButtonClicked;
        BuildingController.Instance.OnBuildingAdded += BuildingAdded;

        buttonScroller = root.Q<ScrollView>("buildingButtonContainer");
        buttonScroller.RegisterCallback<GeometryChangedEvent>(_ => UpdateButtons());
        buttonScroller.RegisterCallback<WheelEvent>(_ => UpdateButtons());

        tooltipContainer = root.Q<VisualElement>("tooltipContainer");

        scrollLeftButton = root.Q<Button>("scrollLeftButton");
        scrollLeftButton.clicked += () => Scroll(-1);
        scrollRightButton = root.Q<Button>("scrollRightButton");
        scrollRightButton.clicked += () => Scroll(1);

        RefreshButtons();
    }

    private void UpdateButtons()
    {
        float scrollX = buttonScroller.scrollOffset.x;
        float contentWidth = buttonScroller.contentContainer.resolvedStyle.width;
        float visibleWidth = buttonScroller.resolvedStyle.width;

        bool canScrollLeft = scrollX > 1f;
        bool canScrollRight = scrollX + visibleWidth + 1 < contentWidth;

        scrollLeftButton.SetEnabled(canScrollLeft);
        scrollRightButton.SetEnabled(canScrollRight);
    }

    void Scroll(int direction)
    {
        float scrollAmount = direction * scrollStep;
        buttonScroller.scrollOffset += new Vector2(scrollAmount, 0f);
        UpdateButtons();
    }

    private void OnDisable()
    {
        BuildingController.Instance.OnBuildingAdded -= BuildingAdded;
    }

    private void BuildingAdded(SmolbeanBuilding building)
    {
        RefreshButtons();
    }

    private void RefreshButtons()
    {
        buttonScroller.Clear();

        foreach (var spec in BuildingController.Instance.BuildableBuildings)
        {
            Button button = new Button();
            button.clickable.clickedWithEventInfo += BuildButtonClicked;
            button.style.backgroundImage = spec.thumbnail;
            button.userData = spec;
            buttonScroller.Add(button);

            button.RegisterCallback<MouseEnterEvent>((e) =>
            {
                var recipePopup = new VisualElement();
                recipePopup.AddToClassList("ingredientTooltip");
                tooltipContainer.Add(recipePopup);
                recipePopup.style.position = Position.Absolute;

                foreach (var ingredient in spec.ingredients)
                {
                    var listItem = new VisualElement();
                    listItem.AddToClassList("ingredientListItem");
                    recipePopup.Add(listItem);

                    var btn = new Button();
                    btn.style.backgroundImage = ingredient.item.thumbnail;
                    listItem.Add(btn);

                    var label = new Label();
                    label.text = "x " + ingredient.quantity;
                    listItem.Add(label);
                }

                var title = new Label();
                title.text = spec.buildingName;
                recipePopup.Add(title);

                recipePopup.RegisterCallback<GeometryChangedEvent>(_ =>
                {
                    Vector2 global = button.worldBound.position;
                    recipePopup.style.left = global.x;
                    recipePopup.style.top = global.y - recipePopup.resolvedStyle.height - 50;
                });

            });
            button.RegisterCallback<MouseLeaveEvent>((e) =>
            {
                tooltipContainer.Clear();
            });
        }
    }

    private void BuildButtonClicked(EventBase eventBase)
    {
        soundPlayer.Play("Click");
        var spec = (BuildingSpec)((Button)eventBase.target).userData;
        MapInteractionManager.Instance.Data.SetStartBuild(spec);
    }

    private void MainMenuButtonClicked()
    {
        MapInteractionManager.Instance.Data.SetCancelled();
        ToolbarController.Instance.ShowToolbar();
    }
}
