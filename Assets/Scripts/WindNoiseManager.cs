using System;
using System.Collections.Generic;
using UnityEngine;

public class WindNoiseManager : MonoBehaviour, IObjectGenerator
{
    public int Priority { get { return 1; } }
    public Texture2D noiseTexture;
    public float noiseScale = 0.1f;

    void Start()
    {
        GenerateNoise();
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        GenerateNoise();
    }

    private void GenerateNoise()
    {
        Debug.Log("Generating wind noise texture");

        int xMid = noiseTexture.width / 2;
        int yMid = noiseTexture.height / 2;
        int xLast = noiseTexture.width - 1;
        int yLast = noiseTexture.height - 1;

        for (int y = 0; y < yMid; y++)
        {
            for (int x = 0; x < xMid; x++)
            {
                float val = Mathf.PerlinNoise((x * 1f / xMid) * noiseScale, (y * 1f / yMid) * noiseScale);
                SetPixel(x, y, val);
                SetPixel(xLast - x, y, val);
                SetPixel(x, yLast - y, val);
                SetPixel(xLast - x, yLast - y, val);
            }
        }

        noiseTexture.Apply();
    }

    private void SetPixel(int x, int y, float val)
    {
        Color c = noiseTexture.GetPixel(x, y);
        // Wind noise is on the GREEN channel
        c.g = val;
        noiseTexture.SetPixel(x, y, c);
    }

    public void Clear()
    {
        Debug.Log("Clearing wind noise texture");
        for (int y = 0; y < noiseTexture.height; y++)
        {
            for (int x = 0; x < noiseTexture.width; x++)
            {
                Color c = noiseTexture.GetPixel(x, y);
                c.g = 0.0f; // Wind noise is on the GREEN channel
                noiseTexture.SetPixel(x, y, c);
            }
        }

        noiseTexture.Apply();
    }
}
