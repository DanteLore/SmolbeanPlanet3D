using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapMenuController : MonoBehaviour
{
    private GridManager gridManager;
    private UIDocument document;
    private VisualElement mapBox;

    public Color wornGroundColor;

    public Color[] mapLevelColors;

    public Texture2D groundTexture;


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
        int width = groundTexture.width;
        int height = groundTexture.height;

        Texture2D mapTexture = new Texture2D(width, height);
        mapTexture.filterMode = FilterMode.Point;

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++) 
            {
                float wear = groundTexture.GetPixel(x, y).r;

                Color baseColor = GetMapColorAt((x * 1f) / width, (y * 1f) / height);
                Color c = Color.Lerp(baseColor, wornGroundColor, wear);
                
                mapTexture.SetPixel(x, y, c);
            }
        }

        mapBox.style.backgroundImage = mapTexture;

        mapTexture.Apply();
    }

    private Color GetMapColorAt(float dX, float dY)
    {
        int x = Mathf.RoundToInt(Mathf.Clamp01(dX) * (gridManager.GameMapWidth - 1));
        int y = Mathf.RoundToInt(Mathf.Clamp01(dY) * (gridManager.GameMapHeight - 1));

        int val = gridManager.GameMap[y * gridManager.GameMapWidth + x];

        return mapLevelColors[val];
    }

    private void CloseButtonClicked()
    {
        MenuController.Instance.CloseAll();
    }
}
