using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;

public class GameMapGenerator : MonoBehaviour
{
    public AnimationCurve islandFalloff;
    public int mapHeight = 100;
    public int mapWidth = 100;
    public int seed = 696809784;

    [Range(0.01f, 0.8f)] public float noiseScale1 = 0.5f;
    [Range(0.01f, 0.5f)] public float noiseScale2 = 0.1f;
    [Range(0.01f, 0.2f)] public float noiseScale3 = 0.05f;

    [Range(0.0f, 1.0f)] public float noiseStength1 = 0.8f;
    [Range(0.0f, 1.0f)] public float noiseStength2 = 0.5f;
    [Range(0.0f, 1.0f)] public float noiseStength3 = 0.1f;

    [Range(-1.0f, 0.5f)] public float heightBias = 0.0f;

    [Range(1, 10)] public int maxLevelNumber = 3;
    [Min(1)]
    public int maxInterLayerStep = 1;

    public List<int> GenerateMap(int seed = 0)
    {
        if (seed != 0)
        {
            this.seed = seed;
        }
        Random.InitState(seed);

        var noise = GenerateNoiseMap();
        var map = MapToLevels(noise);
        map = CleanMap(map);

        return map;
    }
    private static readonly (int x, int y)[] neighbours = new(int, int)[] 
    { 
        (-1, -1), (-1, 0), (-1, 1), 
        (0, -1),           (0, 1), 
        (1, -1),  (1, 0),  (1, 1) 
    };

    private List<int> CleanMap(List<int> m)
    {
        var map = new List<int>(m);
        while(true)
        {
            int errorsFound = 0;

            for (int centerY = 1; centerY < mapWidth - 1; centerY++)
            {
                for (int centerX = 1; centerX < mapHeight - 1; centerX++)
                {
                    int centerHeight = map[(centerY * mapWidth) + centerX];

                    foreach(var (x, y) in neighbours.Select(n => (centerX + n.x, centerY + n.y)))
                    {
                        int neighbourHeight = map[(y * mapWidth) + x];
                        if(centerHeight - neighbourHeight > maxInterLayerStep)
                        {
                            errorsFound++;
                            map[(centerY * mapWidth) + centerX] = Mathf.Max(1, centerHeight - 1); // Lower center square by one, to flatten this area out
                        }
                    }
                }
            }

            if(errorsFound == 0)
                break;
        }

        return map;
    }

    private List<int> MapToLevels(List<float> noise)
    {
        return noise.Select(s => Mathf.FloorToInt(Mathf.Lerp(0.0f, maxLevelNumber + 1.0f, s))).Select(x => Mathf.Min(maxLevelNumber, x)).ToList();
    }

    private List<float> GenerateNoiseMap()
    {
        Vector2 center = new Vector2(mapWidth / 2.0f, mapHeight / 2.0f);
        float maxDist = Mathf.Min(mapWidth, mapHeight) / 2.0f;
        var noiseMap = new List<float>();

        // Offset the perlin noise, because otherwise it's the same every run!
        float xOffset1 = Random.Range(0.0f, 1000.0f);
        float yOffset1 = Random.Range(0.0f, 1000.0f);
        float xOffset2 = Random.Range(0.0f, 1000.0f);
        float yOffset2 = Random.Range(0.0f, 1000.0f);
        float xOffset3 = Random.Range(0.0f, 1000.0f);
        float yOffset3 = Random.Range(0.0f, 1000.0f);

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
