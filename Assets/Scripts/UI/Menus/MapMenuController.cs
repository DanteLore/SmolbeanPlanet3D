using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Collections.Generic;
using System;

public class MapMenuController : SmolbeanMenu
{
    [Serializable]
    public class GameObjectMapEntry
    {
        public GameObject parentObject;
        public Color color;
        public int radius = 3;
    }

    private GridManager gridManager;
    private UIDocument document;
    private VisualElement mapBox;

    public Color wornGroundColor;
    
    public GameObjectMapEntry[] mapEntries;

    public Color[] mapLevelColors;

    public Texture2D groundTexture;
    Texture2D mapTexture;
    public float mapWidth = 404f;
    public float mapHeight = 404f;
    public float mapOffsetX = -202f;
    public float mapOffsetY = -202f;

    void OnEnable()
    {
        gridManager = FindObjectOfType<GridManager>();
        
        document = GetComponent<UIDocument>();

        mapBox = document.rootVisualElement.Q<VisualElement>("mapBox");
        
        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        mapTexture = new Texture2D(groundTexture.width, groundTexture.height);
        mapTexture.filterMode = FilterMode.Point;
        mapBox.style.backgroundImage = mapTexture;
        InvokeRepeating("GenerateMapTexture", 0.1f, 5.0f);
    }

    void OnDisable()
    {
        CancelInvoke("GenerateMapTexture");
    }

    private void GenerateMapTexture()
    {
        Debug.Log("Drawing map");

        DrawMap(mapTexture);

        mapTexture.Apply();
    }

    private void DrawMap(Texture2D mapTexture)
    {
        int width = groundTexture.width;
        int height = groundTexture.height;

        // Map
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

        // Objects
        foreach(var entry in mapEntries)
        {
            DrawNatureObjects(mapTexture, entry.parentObject.transform, entry.color, entry.radius);
        }
    }

    private void DrawNatureObjects(Texture2D mapTexture, Transform parent, Color color, int radius)
    {
        foreach(Transform child in parent)
        {
            var pos = child.position;

            float mapX = (pos.x - mapOffsetX) / mapWidth;
            float mapY = (pos.z - mapOffsetY) / mapHeight;

            int centerX = Mathf.RoundToInt(mapTexture.width * mapX);
            int centerY = Mathf.RoundToInt(mapTexture.height * mapY);

            Vector2Int center = new Vector2Int(centerX, centerY);

            for (int y = centerY - radius; y < centerY + radius; y++)
            {
                for (int x = centerX - radius; x < centerX + radius; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    
                    mapTexture.SetPixel(x, y, color);
                }
            }
        }
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
