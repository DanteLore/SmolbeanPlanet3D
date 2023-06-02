using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapMenuController : MonoBehaviour
{
    private GridManager gridManager;
    private UIDocument document;
    private VisualElement mapBox;

    public Color seaColor;
    public Color lowGroundColor;
    public Color highGroundColor;


    void OnEnable()
    {
        gridManager = FindObjectOfType<GridManager>();
        
        document = GetComponent<UIDocument>();

        mapBox = document.rootVisualElement.Q<VisualElement>("mapBox");
        
        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        DrawMap();
    }

    private void DrawMap()
    {
        Texture2D texture = new Texture2D(gridManager.GameMapWidth, gridManager.GameMapHeight);
        texture.filterMode = FilterMode.Point;

        // Walk y backwards, because textures start in the top left
        for (int y = gridManager.GameMapHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridManager.GameMapWidth; x++)
            {
                float i = gridManager.GameMap[y * gridManager.GameMapWidth + x];

                Color color = seaColor;

                if (i == 2)
                {
                    color = highGroundColor;
                }
                else if (i == 1)
                {
                    color = lowGroundColor;
                }

                texture.SetPixel(x, y, color);
            }
        }

        mapBox.style.backgroundImage = texture;

        texture.Apply();
    }

    private void CloseButtonClicked()
    {
        MenuController.Instance.CloseAll();
    }
}
