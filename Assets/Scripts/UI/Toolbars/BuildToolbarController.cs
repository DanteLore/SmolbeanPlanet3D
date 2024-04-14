using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BuildToolbarController : MonoBehaviour
{
    private UIDocument document;
    private SoundPlayer soundPlayer;

    private void Start()
    {
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
    }

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainToolbarButton");
        mainMenuButton.clicked += MainMenuButtonClicked;
        
        var buttonContainer = document.rootVisualElement.Q<VisualElement>("buildingButtonContainer");
        buttonContainer.Clear();

        foreach(var spec in BuildingController.Instance.buildings.Where(b => b.showInToolbar))
        {
            Button button = new Button();
            button.clickable.clickedWithEventInfo += BuildButtonClicked;
            button.style.backgroundColor = new Color(0, 0, 0, 0);
            button.style.backgroundImage = spec.thumbnail;
            button.userData = spec;
            buttonContainer.Add(button);

            var recipePopup = new VisualElement();
            recipePopup.AddToClassList("ingredientTooltip");
            recipePopup.visible = false;

            foreach(var ingredient in spec.ingredients)
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
            button.RegisterCallback<MouseEnterEvent>((e) => { recipePopup.visible = true; } );
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
