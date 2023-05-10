using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMapGenerator : MonoBehaviour
{
    public GameObject previewPlane;
    public AnimationCurve islandFalloff;
    public int mapHeight = 100;
    public int mapWidth = 100;

    public float noiseScale1 = 0.3f;
    public float noiseScale2 = 0.1f;

    public MapData mapData;

    public List<int> GameMap 
    { 
        get 
        { 
            return mapData.GameMap.ToList(); 
        }
        set
        {
            mapData.SetGameMap(value.ToArray());
        }
    }

    void Start()
    {
        if(previewPlane != null)
        {
            previewPlane.SetActive(false);
        }
    }

    public void GenerateMap()
    {
        var noise = GenerateNoiseMap();
        GameMap = noise.Select(s => Mathf.FloorToInt(Mathf.Lerp(0.0f, 3f, s))).Select(x => x > 2 ? 2 : x).ToList();
        PreviewMap(GameMap);
    }

    public void HidePreview()
    {
        previewPlane.SetActive(false);
    }

    private List<float> GenerateNoiseMap()
    {
        Vector2 center = new Vector2(mapWidth / 2.0f, mapHeight / 2.0f);
        float maxDist = Mathf.Min(mapWidth, mapHeight) / 2.0f;
        var noiseMap = new List<float>();

        // Offset the perlin noise, because otherwise it's the same every run!
        float xOffset = Random.Range(0.0f, 1000.0f);
        float yOffset = Random.Range(0.0f, 1000.0f);

        // Generate noise
        float max = 0f;
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                // Two layers of Perlin noise
                float sample = Mathf.PerlinNoise((x + xOffset) / (mapWidth * noiseScale1), (y + yOffset) / (mapHeight * noiseScale1));
                sample += Mathf.PerlinNoise((x + xOffset) / (mapWidth * noiseScale2), (y + yOffset) / (mapHeight * noiseScale2));

                // Dropoff from centre
                var pos = new Vector2(x, y);
                float dist = Vector2.Distance(pos, center);
                sample *= islandFalloff.Evaluate(dist / maxDist);

                if (sample > max)
                    max = sample;

                noiseMap.Add(sample);
            }
        }

        // Normalise WRT maximum
        return noiseMap.Select(f => f / max).ToList();
    }

    private void PreviewMap(List<int> map)
    {
        if(previewPlane == null)
            return;
     
        previewPlane.SetActive(true);

        Texture2D texture = new Texture2D(mapWidth, mapHeight);
        texture.filterMode = FilterMode.Point;

        var material = previewPlane.GetComponent<Renderer>().sharedMaterial;
        material.mainTexture = texture;
        material.SetFloat("_Smoothness", 0.0f);

        // Walk y backwards, because textures start in the top left
        for (int y = mapWidth - 1; y >= 0; y--)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                float i = map[y * mapWidth + x];

                Color color = Color.blue;

                if (i == 2)
                {
                    color = new Color(0f, 1.0f, 0f);
                }
                else if (i == 1)
                {
                    color = new Color(0f, 0.75f, 0f);
                }

                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
    }
}
