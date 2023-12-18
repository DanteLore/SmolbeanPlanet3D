using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BuildToolbarController : MonoBehaviour
{
    private UIDocument document;

    void OnEnable()
    {
        document = GetComponent<UIDocument>();
        
        var mainMenuButton = document.rootVisualElement.Q<Button>("mainToolbarButton");
        mainMenuButton.clicked += MainMenuButtonClicked;
        
        var buttonContainer = document.rootVisualElement.Q<VisualElement>("buildingButtonContainer");
        buttonContainer.Clear();

        foreach(var spec in BuildManager.Instance.buildings.Where(b => b.showInToolbar))
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
            
            button.Add(recipePopup);
            button.style.overflow = Overflow.Visible;
            //button.RegisterCallback<MouseEnterEvent>((e) => { recipePopup.visible = true; Debug.Log("Mouse over"); } );
            //button.RegisterCallback<MouseLeaveEvent>((e) => { recipePopup.visible = false; Debug.Log("Mouse out"); });
        }
    }

    private void BuildButtonClicked(EventBase eventBase)
    {
        var spec = (BuildingSpec)((Button)eventBase.target).userData;
        BuildManager.Instance.BeginBuild(spec);
    }

    private void MainMenuButtonClicked()
    {
        BuildManager.Instance.EndBuild();
        ToolbarController.Instance.ShowToolbar();
    }
}
