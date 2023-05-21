using System.Collections.Generic;
using UnityEngine;

public class GroundWearManager : MonoBehaviour, IObjectGenerator
{
    public static GroundWearManager Instance { get; private set; }

    public int Priority { get { return 0; }}

    public Texture2D wearTexture;
    public Material grassMaterial;
    public int wearStrength = 2;
    public float updateThreshold = 0.5f;
    public int squaresToGrowBackEachFrame = 50;
    public int wearRadius = 3;

    public float mapWidth = 400f;
    public float mapHeight = 400f;
    public float mapOffsetX = -200f;
    public float mapOffsetY = -200f;

    private int textureWidth;
    private int textureHeight;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        textureWidth = wearTexture.width;
        textureHeight = wearTexture.height;

        Clear();

        InvokeRepeating("UpdateTexture", 1.0f, 0.5f);
    }

    void FixedUpdate()
    {
        float amount = wearStrength / 128f;
        for(int i = 0; i < squaresToGrowBackEachFrame; i++)
        {
            int x = UnityEngine.Random.Range(0, textureWidth);
            int y = UnityEngine.Random.Range(0, textureHeight);

            var px = wearTexture.GetPixel(x, y);
            float r = Mathf.Clamp01(px.r - amount);
            wearTexture.SetPixel(x, y, new Color(r, px.g, px.b));
        }
    }

    public void WalkedOn(Vector3 position)
    {
        WearCircle(WorldToTexture(position), wearRadius);
    }

    private void WearCircle(Vector2Int center, int radius)
    {
        for (int y = Mathf.Max(center.y - radius, 0); y < Mathf.Min(center.y + radius, textureHeight); y++)
        {
            for (int x = Mathf.Max(center.x - radius, 0); x < Mathf.Min(center.x + radius, textureWidth); x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                if (dist <= radius)
                {
                    float c = wearStrength * ((radius - dist) / 256f);
                    var px = wearTexture.GetPixel(x, y);
                    px.r = Mathf.Clamp01(px.r + c); // Wear is on the RED channel
                    wearTexture.SetPixel(x, y, px);
                }
            }
        }
    }

    public void BuildingOn(Bounds bounds, BuildingWearPattern wearPattern, Vector2 wearScale)
    {
        var min = WorldToTexture(bounds.min);
        var max = WorldToTexture(bounds.max);
        var center = WorldToTexture(bounds.center);

        if(wearPattern == BuildingWearPattern.Rectangle)
        {
            int dx = Mathf.RoundToInt((max.x - min.x) * Mathf.Clamp01(1f - wearScale.x));
            int dy = Mathf.RoundToInt((max.y - min.y) * Mathf.Clamp01(1f - wearScale.y));
            min = new Vector2Int(min.x + dx, min.y + dy);
            max = new Vector2Int(max.x + dx, max.y + dy);

            WearRectangle(min, max, center);
        }
        else // Circle 
        {
            int radius = Mathf.RoundToInt(Vector2Int.Distance(min, center) * Mathf.Min(wearScale.x, wearScale.y));
            WearCircle(center, radius);
        }
    }

    private void WearRectangle(Vector2Int min, Vector2Int max, Vector2Int center)
    {
        float radius = Vector2Int.Distance(min, center);

        for (int y = min.y; y < max.y; y++)
        {
            for (int x = min.x; x < max.x; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                
                float c = wearStrength * ((radius - dist) / 256f);
                var px = wearTexture.GetPixel(x, y);
                px.r = Mathf.Clamp01(px.r + c); // Wear is on the RED channel
                wearTexture.SetPixel(x, y, px);
            }
        }
    }

    private Vector2Int WorldToTexture(Vector3 world)
    {
        float mapX = (world.x - mapOffsetX) / mapWidth;
        float mapZ = (world.z - mapOffsetY) / mapHeight;

        return new Vector2Int(Mathf.CeilToInt(textureWidth * mapX), Mathf.CeilToInt(textureHeight * mapZ));
    }

    private void UpdateTexture()
    {
        wearTexture.Apply();
    }

    public void Clear()
    {
        CancelInvoke("UpdateTexture");

        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                Color c = wearTexture.GetPixel(x, y);
                c.r = 0.0f; // Wear is on the RED channel
                wearTexture.SetPixel(x, y, c);
            }
        }

        wearTexture.Apply();
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        InvokeRepeating("UpdateTexture", 1.0f, 0.5f);
    }
}
