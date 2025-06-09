using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

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
    private SoundPlayer soundPlayer;
    private UIDocument document;
    private VisualElement mapBox;

    public Color wornGroundColor;

    public GameObjectMapEntry[] mapEntries;

    public Color seaColor;
    public Gradient landColors;

    public Texture2D groundTexture;
    Texture2D mapTexture;
    public float mapWidth = 404f;
    public float mapHeight = 404f;
    public float mapOffsetX = -202f;
    public float mapOffsetY = -202f;

    void OnEnable()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();

        document = GetComponent<UIDocument>();

        mapBox = document.rootVisualElement.Q<VisualElement>("mapBox");

        var closeButton = document.rootVisualElement.Q<Button>("closeButton");
        closeButton.clicked += CloseButtonClicked;

        mapTexture = new Texture2D(groundTexture.width, groundTexture.height)
        {
            filterMode = FilterMode.Point
        };
        mapBox.style.backgroundImage = mapTexture;

        Debug.Log("Drawing map");
        DrawMap(mapTexture);
    }

    private void DrawMap(Texture2D mapTexture)
    {
        int gameMapWidth = gridManager.GameMapWidth;
        int gameMapHeight = gridManager.GameMapHeight;

        int width = groundTexture.width;
        int height = groundTexture.height;
        Color[] data = groundTexture.GetPixels();

        // Map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float wear = data[y * width + x].r;
                Color baseColor = GetMapColorAt(x * 1f / width, y * 1f / height, gameMapWidth, gameMapHeight);
                Color c = Color.Lerp(baseColor, wornGroundColor, wear);

                data[y * width + x] = c;
            }
        }

        // Objects
        foreach (var entry in mapEntries)
        {
            DrawNatureObjects(data, width, entry.parentObject.transform, entry.color, entry.radius);
        }

        mapTexture.SetPixels(data);
        mapTexture.Apply();
    }

    private void DrawNatureObjects(Color[] data, int width, Transform parent, Color color, int radius)
    {
        float rSquared = radius * radius;

        foreach (Transform child in parent)
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
                    float dist = (new Vector2(x, y) - center).sqrMagnitude;
                    if (dist <= rSquared)
                    {
                        data[y * width + x] = color;
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Color GetMapColorAt(float dX, float dY, int width, int height)
    {
        int x = Mathf.RoundToInt(dX * (width - 1));
        int y = Mathf.RoundToInt(dY * (height - 1));

        int val = gridManager.GameMap[y * width + x];

        if (val == 0)
        {
            return seaColor;
        }
        else
        {
            float a = Mathf.InverseLerp(0f, gridManager.MaxLevelNumber - 1, val - 1);
            return landColors.Evaluate(a);
        }
    }

    private void CloseButtonClicked()
    {
        soundPlayer.Play("Click");
        MenuController.Instance.CloseAll();
    }
}