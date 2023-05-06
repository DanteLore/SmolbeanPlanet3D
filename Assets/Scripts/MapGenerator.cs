using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapGenerator 
{
    private int drawMapWidth;
    private int drawMapHeight;
    private int gameMapWidth;
    private int gameMapHeight;
    private float coastRadius = 0.8f;
    private Dictionary<String, MeshData> meshData;
    private Dictionary<String, NeighbourData> neighbourData;
    private System.Random rand = new System.Random();
    private List<string> allTileOptions;
    private Dictionary<string, double> tileProbabilities;
    private Vector2 mapCentre;

    public MapGenerator(int gameMapWidth, int gameMapHeight, float coastRadius, List<MeshData> meshData, Dictionary<String, NeighbourData> neighbourData)
    {
        this.gameMapWidth = gameMapWidth;
        this.gameMapHeight = gameMapHeight;
        this.drawMapWidth = gameMapWidth + 1;
        this.drawMapHeight = gameMapHeight + 1;
        this.coastRadius = coastRadius;
        this.meshData = meshData.ToDictionary(md => md.name, md => md);
        this.neighbourData = neighbourData;

        allTileOptions = neighbourData.Keys.ToList();
        mapCentre = new Vector2(drawMapWidth / 2.0f, drawMapHeight / 2.0f);
    }

    public MeshData[] GenerateMap(int[] gameMap)
    {
        MapSquareOptions[] drawMap = InitialiseMap();

        ApplyGameMapRestrictions(gameMap, drawMap);
        AddOcean(drawMap);

        while (drawMap.Any(ms => !ms.IsCollapsed))
        {
            // Select square that isn't collapsed yet with lowest possibilities
            var target = drawMap.Where(ms => !ms.IsCollapsed).OrderBy(ms => ms.NumberOfPossibilities).ThenByDescending(DistanceFromMapCentre).First();
            string tile = SelectWeightedRandomTile(target);
            target.Choose(tile);

            // Collapse outward - recurse until stable
            CollapseAt(target, drawMap);
        }

        var tiles = drawMap.Select(m => meshData[m.TileName]).ToArray();

        return tiles;
    }

    private void ApplyGameMapRestrictions(int[] gameMap, MapSquareOptions[] drawMap)
    {
        for(int y = 0; y < gameMapHeight; y++)
        {
            for(int x = 0; x < gameMapWidth; x++)
            {
                int level = gameMap[y * gameMapWidth + x];

                var backLeft = drawMap[(y + 1) * drawMapWidth + x];
                var backRight = drawMap[(y + 1) * drawMapWidth + x + 1];
                var frontLeft = drawMap[y * drawMapWidth + x];
                var frontRight = drawMap[y * drawMapWidth + x + 1];

                string[] allowedBackLeft = allTileOptions.Where(t => neighbourData[t].frontRightLevel == level).ToArray();
                string[] allowedBackRight = allTileOptions.Where(t => neighbourData[t].frontleftLevel == level).ToArray();
                string[] allowedFrontLeft = allTileOptions.Where(t => neighbourData[t].backRightLevel == level).ToArray();
                string[] allowedFrontRight = allTileOptions.Where(t => neighbourData[t].backLeftLevel == level).ToArray();

                backLeft.Restrict(allowedBackLeft);
                CollapseAt(backLeft, drawMap);

                backRight.Restrict(allowedBackRight);
                CollapseAt(backRight, drawMap);

                frontLeft.Restrict(allowedFrontLeft);
                CollapseAt(frontLeft, drawMap);

                frontRight.Restrict(allowedFrontRight);
                CollapseAt(frontRight, drawMap);
            }
        }
    }

    private float DistanceFromMapCentre(MapSquareOptions ms)
    {
        return (new Vector2(ms.X, ms.Y) - mapCentre).magnitude;
    }

    private MapSquareOptions[] InitialiseMap()
    {
        var drawMap = new MapSquareOptions[drawMapHeight * drawMapWidth];
        for (int y = 0; y < drawMapHeight; y++)
            for (int x = 0; x < drawMapWidth; x++)
                drawMap[y * drawMapWidth + x] = new MapSquareOptions(x, y, allTileOptions);
        return drawMap;
    }

    private void AddOcean(MapSquareOptions[] drawMap)
    {
        float radius = (Mathf.Min(drawMapHeight, drawMapWidth) / 2) * coastRadius;

        for (int y = 0; y < drawMapHeight; y++)
        {
            for (int x = 0; x < drawMapWidth; x++)
            {
                Vector2 pos = new Vector2(x, y);

                if (Vector2.Distance(pos, mapCentre) > radius)
                {
                    var target = drawMap[y * drawMapWidth + x];
                    target.Choose("Seabed");
                    CollapseAt(target, drawMap);
                }
            }
        }
    }

    private string SelectWeightedRandomTile(MapSquareOptions target)
    {
        // Add some noise to the values so we get a random sort order and break ties between items with the same weighting in a random way
        // Order by priority, biggest first
        float noiseWeight = 1f;
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

    private void CollapseAt(MapSquareOptions target, MapSquareOptions[] drawMap)
    {
        List<MapSquareOptions> recurseInto = new List<MapSquareOptions>();
        
        // Left
        if(target.X > 0)
        {
            var left = drawMap[target.Y * drawMapWidth + target.X - 1];
            var allowed = target.Options.SelectMany(o => neighbourData[o].leftMatches).Distinct();

            if(left.Restrict(allowed))
                recurseInto.Add(left);
        }
        
        // Right
        if(target.X < drawMapWidth - 1)
        {
            var right = drawMap[target.Y * drawMapWidth + target.X + 1];
            var allowed = target.Options.SelectMany(o => neighbourData[o].rightMatches).Distinct();

            if(right.Restrict(allowed))
                recurseInto.Add(right);
        }
        
        // Front
        if(target.Y > 0)
        {
            var front = drawMap[(target.Y - 1) * drawMapWidth + target.X];
            var allowed = target.Options.SelectMany(o => neighbourData[o].frontMatches).Distinct();

            if(front.Restrict(allowed))
                recurseInto.Add(front);
        }
        
        // Back
        if(target.Y < drawMapHeight - 1)
        {
            var back = drawMap[(target.Y + 1) * drawMapWidth + target.X];
            var allowed = target.Options.SelectMany(o => neighbourData[o].backMatches).Distinct();

            if(back.Restrict(allowed))
                recurseInto.Add(back);
        }

        recurseInto = recurseInto.ToList();
        foreach(var square in recurseInto)
            CollapseAt(square, drawMap);
    }
}
