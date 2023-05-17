using System.Collections;
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

    void Update()
    {
        //int x = UnityEngine.Random.Range(0, textureWidth);
        //int y = UnityEngine.Random.Range(0, textureHeight);

        //wearTexture.SetPixel(x, y, Color.white);
    }

    public void WalkedOn(Vector3 position)
    {
        float mapX = (position.x - mapOffsetX) / mapWidth;
        float mapZ = (position.z - mapOffsetY) / mapHeight;

        var center = new Vector2Int(Mathf.RoundToInt(textureWidth * mapX), Mathf.RoundToInt(textureWidth * mapZ));

        //Debug.Log($"{position.x} - {mapBounds.min.x} / {mapBounds.size.x} == {mapX}");

        int radius = 3;

        for(int y = Mathf.Max(center.y - radius, 0); y < Mathf.Min(center.y + radius, textureHeight); y++)
        {

            for(int x = Mathf.Max(center.x - radius, 0); x < Mathf.Min(center.x + radius, textureWidth); x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);

                if(dist <= radius)
                {
                    float c = wearStrength * ((radius - dist) / 255);

                    //Debug.Log($"float {c} = {wearStrength} * ({radius} - {dist}) / {radius}");

                    var px = wearTexture.GetPixel(x, y);
                    wearTexture.SetPixel(x, y, new Color(px.r + c, 0f, 0f));
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
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                wearTexture.SetPixel(x, y, Color.black);
            }
        }

        wearTexture.Apply();
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        // Nothing to do here
    }
}
