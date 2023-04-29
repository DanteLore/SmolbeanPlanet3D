using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapGenerator 
{
    private int mapWidth;
    private int mapHeight;
    private float coastRadius = 0.8f;
    private Dictionary<String, MeshData> meshData;
    private Dictionary<String, NeighbourData> neighbourData;
    private System.Random rand = new System.Random();
    private List<string> allTileOptions;
    private Dictionary<string, double> tileProbabilities;
    private Vector2 mapCentre;

    public MapGenerator(int mapWidth, int mapHeight, float coastRadius, List<MeshData> meshData, Dictionary<String, NeighbourData> neighbourData)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.coastRadius = coastRadius;
        this.meshData = meshData.ToDictionary(md => md.name, md => md);
        this.neighbourData = neighbourData;

        allTileOptions = neighbourData.Keys.ToList();
        mapCentre = new Vector2(mapWidth / 2, mapHeight / 2);
    }

    public MeshData[] GenerateMap()
    {
        MapSquareOptions[] map = InitialiseMap();
        InitialiseTileProbabilities();
        AddOcean(map);
        while (map.Any(ms => !ms.IsCollapsed))
        {
            // Select square that isn't collapsed yet with lowest possibilities
            var target = map.Where(ms => !ms.IsCollapsed).OrderBy(ms => ms.NumberOfPossibilities).ThenBy(ms => (new Vector2(ms.X, ms.Y) - mapCentre).magnitude).First();
            string tile = SelectWeightedRandomTile(target);
            target.Choose(tile);

            // Collapse outward - recurse until stable
            CollapseAt(target, map);
        }

        var tiles = map.Select(m => meshData[m.TileName]).ToArray();

        return tiles;
    }

    private MapSquareOptions[] InitialiseMap()
    {
        var map = new MapSquareOptions[mapHeight * mapWidth];
        for (int y = 0; y < mapHeight; y++)
            for (int x = 0; x < mapWidth; x++)
                map[y * mapWidth + x] = new MapSquareOptions(x, y, allTileOptions);
        return map;
    }

    private void AddOcean(MapSquareOptions[] map)
    {
        float radius = (Mathf.Min(mapHeight, mapWidth) / 2) * coastRadius;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector2 pos = new Vector2(x, y);

                if (Vector2.Distance(pos, mapCentre) > radius)
                {
                    var target = map[y * mapWidth + x];
                    target.Choose("SeaFloor");
                    CollapseAt(target, map);
                }
            }
        }
    }

    private void InitialiseTileProbabilities()
    {
        tileProbabilities = new Dictionary<string, double>();
        foreach (var tileName in allTileOptions)
        {
            double p = 1.0;

            if (tileName.ToLower().Contains("sea"))
                p *= 0.5;

            if (tileName.ToLower().Contains("corner"))
                p *= 0.5;

            if (tileName.ToLower().Contains("floor"))
                p *= 2.0;

            tileProbabilities.Add(tileName, p);
        }
    }

    private string SelectWeightedRandomTile(MapSquareOptions target)
    {
        // Add some noise to the values so we get a random sort order and break ties between items with the same weighting in a random way
        // Order by priority, biggest first
        float noiseWeight = 0.001f;
        var choices = target.Options.Select(o => (P: tileProbabilities[o] + rand.NextDouble() * noiseWeight, I: o)).OrderByDescending(c => c.P).ToList();
        
        double max = choices.Sum(c => c.P);
        double cutoff = rand.NextDouble() * max;
        double sum = 0;

        // Walk down the list until we pass the randomly selected cutoff.  Return that index.
        foreach (var c in choices)
        {
            sum += c.P;
            if (sum > cutoff)
                return c.I;
        }

        // Must be the last item...
        return choices.Last().I;
    }

    private void CollapseAt(MapSquareOptions target, MapSquareOptions[] map)
    {
        List<MapSquareOptions> recurseInto = new List<MapSquareOptions>();
        
        // Left
        if(target.X > 0)
        {
            var left = map[target.Y * mapWidth + target.X - 1];
            var allowed = target.Options.SelectMany(o => neighbourData[o].leftMatches).Distinct();

            if(left.Restrict(allowed))
                recurseInto.Add(left);
        }
        
        // Right
        if(target.X < mapWidth - 1)
        {
            var right = map[target.Y * mapWidth + target.X + 1];
            var allowed = target.Options.SelectMany(o => neighbourData[o].rightMatches).Distinct();

            if(right.Restrict(allowed))
                recurseInto.Add(right);
        }
        
        // Front
        if(target.Y > 0)
        {
            var front = map[(target.Y - 1) * mapWidth + target.X];
            var allowed = target.Options.SelectMany(o => neighbourData[o].frontMatches).Distinct();

            if(front.Restrict(allowed))
                recurseInto.Add(front);
        }
        
        // Back
        if(target.Y < mapHeight - 1)
        {
            var back = map[(target.Y + 1) * mapWidth + target.X];
            var allowed = target.Options.SelectMany(o => neighbourData[o].backMatches).Distinct();

            if(back.Restrict(allowed))
                recurseInto.Add(back);
        }

        recurseInto = recurseInto.ToList();
        foreach(var square in recurseInto)
            CollapseAt(square, map);
    }
}
