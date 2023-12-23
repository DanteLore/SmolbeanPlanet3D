using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameMapGenerator : MonoBehaviour
{
    public AnimationCurve islandFalloff;
    public int mapHeight = 100;
    public int mapWidth = 100;

    [Range(0.0f, 0.5f)] public float noiseScale1 = 0.3f;
    [Range(0.0f, 0.5f)] public float noiseScale2 = 0.1f;
    [Range(0.0f, 0.5f)] public float noiseScale3 = 0.05f;

    [Range(0.0f, 1.0f)] public float noiseStength1 = 0.8f;
    [Range(0.0f, 1.0f)] public float noiseStength2 = 0.5f;
    [Range(0.0f, 1.0f)] public float noiseStength3 = 0.1f;

    [Range(-1.0f, 0.5f)] public float heightBias = 0.0f;

    [Range(1, 10)] public int maxLevelNumber = 3;

    public int seed = 696809784;

    public List<int> GenerateMap(int seed = 0)
    {
        if(seed != 0)
        {
            this.seed = seed;
        }
        Random.InitState(seed);

        var noise = GenerateNoiseMap();
        return noise.Select(s => Mathf.FloorToInt(Mathf.Lerp(0.0f, maxLevelNumber + 1.0f, s))).Select(x => Mathf.Min(maxLevelNumber, x)).ToList();
    }

    private List<float> GenerateNoiseMap()
    {
        Vector2 center = new Vector2(mapWidth / 2.0f, mapHeight / 2.0f);
        float maxDist = Mathf.Min(mapWidth, mapHeight) / 2.0f;
        var noiseMap = new List<float>();

        // Offset the perlin noise, because otherwise it's the same every run!
        float xOffset1 = UnityEngine.Random.Range(0.0f, 1000.0f);
        float yOffset1 = UnityEngine.Random.Range(0.0f, 1000.0f);
        float xOffset2 = UnityEngine.Random.Range(0.0f, 1000.0f);
        float yOffset2 = UnityEngine.Random.Range(0.0f, 1000.0f);
        float xOffset3 = UnityEngine.Random.Range(0.0f, 1000.0f);
        float yOffset3 = UnityEngine.Random.Range(0.0f, 1000.0f);

        // Generate noise
        float max = 0f;
        for (int y = 0; y < mapWidth; y++)
        {
            for (int x = 0; x < mapHeight; x++)
            {
                // Three layers of Perlin noise
                float part1 = Mathf.PerlinNoise((x + xOffset1) / (mapWidth * noiseScale1), (y + yOffset1) / (mapHeight * noiseScale1));
                float part2 = Mathf.PerlinNoise((x + xOffset2) / (mapWidth * noiseScale2), (y + yOffset2) / (mapHeight * noiseScale2));
                float part3 = Mathf.PerlinNoise((x + xOffset3) / (mapWidth * noiseScale3), (y + yOffset3) / (mapHeight * noiseScale3));

                float sample = noiseStength1 * part1 + noiseStength2 * part2 + noiseStength3 * part3;
                sample += heightBias;

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
}
