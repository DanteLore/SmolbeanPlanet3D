using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GroundWearManager : MonoBehaviour, IObjectGenerator
{
    public static GroundWearManager Instance { get; private set; }

    public int Priority { get { return 2; }}

    public bool RunModeOnly { get { return true; } }

    public Texture2D wearTexture;
    public Material grassMaterial;
    public float textureUpdateDelay = 0.25f;
    public float wearStrength = 1 / 255;
    public float updateThreshold = 0.5f;
    public int squaresToGrowBackEachFrame = 1024;
    public float grassGrowthWeight = 0.5f;
    public int wearRadius = 3;

    public float mapWidth = 400f;
    public float mapHeight = 400f;
    public float mapOffsetX = -200f;
    public float mapOffsetY = -200f;

    private Color32[] data;
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

        InvokeRepeating(nameof(UpdateTexture), 1.0f, textureUpdateDelay);
    }

    void Update()
    {
        GrowGrass();
    }

    private int grassGrowthBatchStart = 0;
    private void GrowGrass()
    {
        // Amount to grow back each frame, scaled by frame time and by number of frames to update a batch
        float amount = Time.deltaTime * grassGrowthWeight * (data.Length / squaresToGrowBackEachFrame);

        int start = grassGrowthBatchStart;
        int end = Mathf.Min(grassGrowthBatchStart + squaresToGrowBackEachFrame, data.Length);

        for (int i = start; i < end; i++)
        {
            Color px = data[i];

            if(px.r > 0f)
            {
                float r = px.r - amount;
                r = r < 0.0f ? 0.0f : r > 1.0f ? 1.0f : r; // faster than clamp01
                data[i].r = (byte)(r * 255);
            }
        }

        grassGrowthBatchStart = end >= data.Length ? 0 : end;
    }

    public float GetAvailableGrass(Vector3 worldPosition, float searchRadius = 0.75f)
    {
        Vector2Int center = WorldToTexture(worldPosition);
        Vector2Int p = WorldToTexture(worldPosition + Vector3.forward * searchRadius);
        int radius = Mathf.CeilToInt(Vector2Int.Distance(center, p));

        int x = center.x;
        int y = center.y;
        float rSquared = radius * radius;
        float grassAvailable = 0f;

        for (int u = x - radius; u < x + radius + 1; u++)
        {  
            for (int v = y - radius; v < y + radius + 1; v++)
            {
                float dSquared = (x - u) * (x - u) + (y - v) * (y - v);
                if (dSquared < rSquared)
                {
                    Color c = data[v * textureWidth + u];
                    grassAvailable += c.g * (1 - c.r); // Green is the grass density, red is the wear level
                }
            }
        }

        return grassAvailable;
    }

    public void WalkedOn(Vector3 position)
    {
        WearCircle(WorldToTexture(position), wearRadius);
    }

    public void RegisterHarvest(Vector3 position, float weight = 1f, float wearRadius = 0.75f)
    {
        Vector2Int center = WorldToTexture(position);
        Vector2Int p = WorldToTexture(position + Vector3.forward * wearRadius);
        int radius = (int)Vector2Int.Distance(center, p);

        WearCircle(center, radius, weight);
    }

    public void BuildingOn(Bounds bounds, BuildingWearPattern wearPattern, Vector2 wearScale)
    {
        if (wearPattern == BuildingWearPattern.Rectangle)
            WearRectangle(bounds, wearScale);
        else
            WearCircle(bounds, wearScale);
    }

    private void WearCircle(Bounds bounds, Vector2 wearScale, float weight = 1.0f)
    {
        var min = WorldToTexture(bounds.min);
        var center = WorldToTexture(bounds.center);
        int radius = (int)((center.x - min.x) * wearScale.x);

        WearCircle(center, radius, weight);
    }

    private void WearCircle(Vector2Int center, int radius, float weight = 1.0f)
    {
        int x = center.x;
        int y = center.y;
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
        {
            for (int v = y - radius; v < y + radius + 1; v++)
            {
                float dSquared = (x - u) * (x - u) + (y - v) * (y - v);
                if (dSquared < rSquared)
                {
                    int index = v * textureWidth + u;
                    Color px = data[index];

                    if(px.r < 1.0f)
                    {
                        float blur = 1 - (dSquared / rSquared);
                        float c = wearStrength * weight * blur;
                        float r = px.r + c; // Wear is on the RED channel
                        r = r < 0.0f ? 0.0f : r > 1.0f ? 1.0f : r; // faster than clamp01?
                        data[index].r = (byte)(r * 255);
                    }
                }
            }
        }
    }

    private void WearRectangle(Bounds bounds, Vector2 wearScale)
    {
        var min = WorldToTexture(bounds.min);
        var center = WorldToTexture(bounds.center);

        float radiusX = center.x - min.x;
        float radiusY = center.y - min.y;

        int startX = Mathf.RoundToInt(center.x - radiusX * wearScale.x);
        int endX = Mathf.RoundToInt(center.x + radiusX * wearScale.x);
        int startY = Mathf.RoundToInt(center.y - radiusY * wearScale.y);
        int endY = Mathf.RoundToInt(center.y + radiusY * wearScale.y);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                int index = y * textureWidth + x;
                Color px = data[index];

                if(px.r < 1.0f)
                {
                    float c = wearStrength * Time.deltaTime;
                    float r = px.r + c; // Wear is on the RED channel
                    r = r < 0.0f ? 0.0f : r > 1.0f ? 1.0f : r; // faster than clamp01?
                    data[index] = new Color(r, px.g, px.b, px.a);
                }
            }
        }
    }

    private Vector2Int WorldToTexture(Vector3 world)
    {
        float mapX = (world.x - mapOffsetX) / mapWidth;
        float mapZ = (world.z - mapOffsetY) / mapHeight;

        return new Vector2Int((int)(textureWidth * mapX), (int)(textureHeight * mapZ));
    }

    private void UpdateTexture()
    {
        wearTexture.SetPixels32(data);
        wearTexture.Apply();
    }

    public void Clear()
    {
        CancelInvoke(nameof(UpdateTexture));

        data = wearTexture.GetPixels32();

        for (int i = 0; i < data.Length; i++)
        {
            // Clear all but the green channel
            data[i].r = 0; 
            data[i].b = 0;
            data[i].a = 0;
        }
        
        UpdateTexture();
    }

    public IEnumerator Load(SaveFileData saveData, string filename)
    {
        if (!string.IsNullOrEmpty(filename))
        {
            string pngFilename = GetPngFilename(filename);
            if (File.Exists(pngFilename) && wearTexture != null)
            {
                wearTexture.LoadImage(File.ReadAllBytes(pngFilename));
            }
        }

        data = wearTexture.GetPixels32();

        InvokeRepeating(nameof(UpdateTexture), 1.0f, textureUpdateDelay);
        yield return null;
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        InvokeRepeating(nameof(UpdateTexture), 1.0f, textureUpdateDelay);
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        if (wearTexture != null)
        {
            string pngFile = GetPngFilename(filename);
            File.WriteAllBytes(pngFile, wearTexture.EncodeToPNG());
        }
    }

    private static string GetPngFilename(string filename)
    {
        return filename.Replace(SaveGameManager.EXTENSION, ".png");
    }
}
