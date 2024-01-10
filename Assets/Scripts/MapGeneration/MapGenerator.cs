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
    private Dictionary<String, MeshData> meshData;
    private Dictionary<int, NeighbourData> neighbourData;
    private List<int> allTileOptions;
    private List<int> seabedOptions;
    private Vector2 mapCentre;
    MapSquareOptions[] drawMap;

    public MapGenerator(int gameMapWidth, int gameMapHeight, List<MeshData> meshData, Dictionary<int, NeighbourData> neighbourData)
    {
        this.gameMapWidth = gameMapWidth;
        this.gameMapHeight = gameMapHeight;
        this.drawMapWidth = gameMapWidth + 1;
        this.drawMapHeight = gameMapHeight + 1;
        this.meshData = meshData.ToDictionary(md => md.name, md => md);
        this.neighbourData = neighbourData;

        allTileOptions = neighbourData.Keys.ToList();
        seabedOptions = this.meshData.Values.Where(x => x.name.StartsWith("Seabed")).Select(x => x.id).ToList();
        mapCentre = new Vector2(drawMapWidth / 2.0f, drawMapHeight / 2.0f);
    }

    public MeshData[] GenerateMap(List<int> gameMap)
    {
        drawMap = InitialiseMap();

        //AddOcean(gameMap);

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

                //Debug.Log($"x={x} y={y} level={level} options={allTileOptions.Count}");

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

    private MapSquareOptions[] InitialiseMap()
    {
        var drawMap = new MapSquareOptions[drawMapHeight * drawMapWidth];
        for (int y = 0; y < drawMapHeight; y++)
        {
            for (int x = 0; x < drawMapWidth; x++)
            {
                // Force the edges of the map to be seabed
                if(x == 0 || y == 0 || x == drawMapWidth - 1 || y == drawMapHeight - 1)
                    drawMap[y * drawMapWidth + x] = new MapSquareOptions(x, y, seabedOptions);
                else
                    drawMap[y * drawMapWidth + x] = new MapSquareOptions(x, y, allTileOptions);
            }
        }
        return drawMap;
    }

    private int SelectRandomTile(MapSquareOptions target)
    {
        return target.Options[UnityEngine.Random.Range(0, target.Options.Count)];
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
