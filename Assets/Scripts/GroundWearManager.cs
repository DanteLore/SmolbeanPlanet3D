using System.Collections.Generic;
using UnityEngine;

public class GroundWearManager : MonoBehaviour, IObjectGenerator
{
    public static GroundWearManager Instance { get; private set; }

    public int Priority { get { return 0; }}

    public Texture2D wearTexture;
    public Material grassMaterial;
    public int wearStrength = 20;
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
        float mapX = (position.x - mapOffsetX) / mapWidth;
        float mapZ = (position.z - mapOffsetY) / mapHeight;

        var center = new Vector2Int(Mathf.RoundToInt(textureWidth * mapX), Mathf.RoundToInt(textureWidth * mapZ));

        for(int y = Mathf.Max(center.y - wearRadius, 0); y < Mathf.Min(center.y + wearRadius, textureHeight); y++)
        {

            for(int x = Mathf.Max(center.x - wearRadius, 0); x < Mathf.Min(center.x + wearRadius, textureWidth); x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                if(dist <= wearRadius)
                {
                    float c = wearStrength * ((wearRadius - dist) / 256f);
                    var px = wearTexture.GetPixel(x, y); 
                    px.r = Mathf.Clamp01(px.r + c); // Wear is on the RED channel
                    wearTexture.SetPixel(x, y, px);
                }
            }
        }
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
