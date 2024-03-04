using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class WaveFunctionCollapse 
{
    private readonly int drawMapWidth;
    private readonly int drawMapHeight;
    private readonly int gameMapWidth;
    private readonly int gameMapHeight;
    private readonly Dictionary<string, MeshData> meshData;
    private readonly Dictionary<int, NeighbourData> neighbourData;
    private readonly int[] allTileOptions;
    private readonly int[] seabedOptions;
    MapSquareOptions[] drawMap;

    public MeshData[] tiles;

    public WaveFunctionCollapse(int gameMapWidth, int gameMapHeight, List<MeshData> meshData, Dictionary<int, NeighbourData> neighbourData)
    {
        this.gameMapWidth = gameMapWidth;
        this.gameMapHeight = gameMapHeight;
        this.drawMapWidth = gameMapWidth + 1;
        this.drawMapHeight = gameMapHeight + 1;
        this.meshData = meshData.ToDictionary(md => md.name, md => md);
        this.neighbourData = neighbourData;

        allTileOptions = neighbourData.Keys.ToArray();
        seabedOptions = this.meshData.Values.Where(x => x.name.StartsWith("Seabed")).Select(x => x.id).ToArray();
    }

    public IEnumerator GenerateMap(List<int> gameMap)
    {
        drawMap = InitialiseMap();
        yield return null;

        yield return ApplyGameMapRestrictions(gameMap);

        int i = 0;
        while (drawMap.Any(ms => !ms.IsCollapsed))
        {
            // Select square that isn't collapsed yet with lowest possibilities
            var target = drawMap.Where(ms => !ms.IsCollapsed).OrderBy(ms => ms.NumberOfPossibilities).First();
            int tile = SelectRandomTile(target);
            target.Choose(tile);

            // Collapse - recurse until stable
            CollapseAt(target);

            if(i++ % 1000 == 0)
                yield return null;
        }

        tiles = drawMap.Select(m => meshData[neighbourData[m.TileId].name]).ToArray();
    }

    private IEnumerator ApplyGameMapRestrictions(List<int> gameMap)
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
        yield return null;

        foreach (var square in recurseInto)
            CollapseAt(square);

        yield return null;
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
        List<MapSquareOptions> recurseInto = new();
        
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
