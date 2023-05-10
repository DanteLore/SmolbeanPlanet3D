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
    private Dictionary<int, NeighbourData> neighbourData;
    private System.Random rand = new System.Random();
    private List<int> allTileOptions;
    private Vector2 mapCentre;
    MapSquareOptions[] drawMap;

    public MapGenerator(int gameMapWidth, int gameMapHeight, float coastRadius, List<MeshData> meshData, Dictionary<int, NeighbourData> neighbourData)
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

    public MeshData[] GenerateMap(List<int> gameMap)
    {
        drawMap = InitialiseMap();

        AddOcean();

        ApplyGameMapRestrictions(gameMap);

        while (drawMap.Any(ms => !ms.IsCollapsed))
        {
            // Select square that isn't collapsed yet with lowest possibilities
            var target = drawMap.Where(ms => !ms.IsCollapsed).OrderBy(ms => ms.NumberOfPossibilities).First();
            int tile = SelectRandomTile(target);
            target.Choose(tile);

            // Collapse - recurse until stable
            CollapseAt(target);
        }

        var tiles = drawMap.Select(m => meshData[neighbourData[m.TileId].name]).ToArray();

        return tiles;
    }

    private void ApplyGameMapRestrictions(List<int> gameMap)
    {
        var recurseInto = new List<MapSquareOptions>();
        for(int y = 0; y < gameMapHeight; y++)
        {
            for(int x = 0; x < gameMapWidth; x++)
            {
                int level = gameMap[y * gameMapWidth + x];

                var backLeft = drawMap[(y + 1) * drawMapWidth + x];
                var backRight = drawMap[(y + 1) * drawMapWidth + x + 1];
                var frontLeft = drawMap[y * drawMapWidth + x];
                var frontRight = drawMap[y * drawMapWidth + x + 1];

                if(backLeft.Restrict(allTileOptions.Where(t => neighbourData[t].frontRightLevel == level)))
                    recurseInto.Add(backLeft);

                if(backRight.Restrict(allTileOptions.Where(t => neighbourData[t].frontleftLevel == level)))
                    recurseInto.Add(backRight);

                if(frontLeft.Restrict(allTileOptions.Where(t => neighbourData[t].backRightLevel == level)))
                    recurseInto.Add(frontLeft);

                if(frontRight.Restrict(allTileOptions.Where(t => neighbourData[t].backLeftLevel == level)))
                    recurseInto.Add(frontRight);
            }
        }

        foreach(var square in recurseInto)
            CollapseAt(square);
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

    private void AddOcean()
    {
        float radius = (Mathf.Min(drawMapHeight, drawMapWidth) / 2) * coastRadius;
        var squaresToCollapse = new List<MapSquareOptions>();

        var seabed = meshData.Values.First(x => x.name == "Seabed").id;

        for (int y = 0; y < drawMapHeight; y++)
        {
            for (int x = 0; x < drawMapWidth; x++)
            {
                Vector2 pos = new Vector2(x, y);

                if (Vector2.Distance(pos, mapCentre) > radius)
                {
                    var target = drawMap[y * drawMapWidth + x];
                    target.Choose(seabed);
                    squaresToCollapse.Add(target);
                }
            }
        }

        foreach(var square in squaresToCollapse)
            CollapseAt(square);
    }

    private int SelectRandomTile(MapSquareOptions target)
    {
        return target.Options[rand.Next(target.Options.Count)];
    }

    private void CollapseAt(MapSquareOptions target)
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

        foreach(var square in recurseInto)
            CollapseAt(square);
    }
}
