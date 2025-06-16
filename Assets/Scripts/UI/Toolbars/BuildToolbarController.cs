using System;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildToolbarController : MonoBehaviour
{
    private SoundPlayer soundPlayer;
    private VisualElement root;
    private ScrollView buttonScroller;
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
        bool canScrollRight = scrollX + visibleWidth + scrollStep < contentWidth;

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
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = spec.thumbnail;
            button.userData = spec;
            buttonScroller.Add(button);

            var recipePopup = new VisualElement();
            recipePopup.AddToClassList("ingredientTooltip");
            recipePopup.visible = false;

            foreach (var ingredient in spec.ingredients)
            {
                var listItem = new VisualElement();
                listItem.AddToClassList("ingredientListItem");
                recipePopup.Add(listItem);

                var btn = new Button();
                btn.style.backgroundColor = new Color(0, 0, 0, 0);
                btn.style.backgroundImage = ingredient.item.thumbnail;
                listItem.Add(btn);

                var label = new Label();
                label.text = "x " + ingredient.quantity;
                listItem.Add(label);
            }

            var title = new Label();
            title.text = spec.buildingName;
            recipePopup.Add(title);

            button.Add(recipePopup);
            button.style.overflow = Overflow.Visible;
            button.RegisterCallback<MouseEnterEvent>((e) => { recipePopup.visible = true; });
            button.RegisterCallback<MouseLeaveEvent>((e) => { recipePopup.visible = false; });
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
