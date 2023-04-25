using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapGenerator 
{
    public class MapSquareOptions
    {
        private List<string> options;
        public int X { get; private set; }
        public int Y { get; private set; }

        public bool IsCollapsed { get { return options.Count == 1; }}

        public String TileName { get { return options.First(); }}

        public int NumberOfPossibilities { get { return options.Count; }}

        public IReadOnlyList<String> Options { get { return options; }}

        public MapSquareOptions(int x, int y, List<String> options)
        {
            this.options = options;
            this.X = x;
            this.Y = y;
        }

        public void Choose(String option)
        {
            options = new List<string> { option };
        }

        public bool Restrict(IEnumerable<String> allowed)
        {
            int cnt = options.Count;
            options = options.Intersect(allowed).ToList();

            if(options.Count == 0)
                throw new Exception("Wave function collapse failed - impossible tile combination found");

            //if(IsCollapsed)
            //    Debug.Log($"Square X{X} Y{Y} restricted to {TileName}");
            //else
            //    Debug.Log($"Square X{X} Y{Y} restricted from {cnt} to {options.Count}");

            return cnt != options.Count;
        }
    }

    private int mapWidth = 10;
    private int mapHeight = 10;
    private Dictionary<String, MeshData> meshData;
    private Dictionary<String, NeighbourData> neighbourData;

    public MapGenerator(int mapWidth, int mapHeight, List<MeshData> meshData, Dictionary<String, NeighbourData> neighbourData)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.meshData = meshData.ToDictionary(md => md.name, md => md);
        this.neighbourData = neighbourData;
    }

    public MeshData[] GenerateMap()
    {
        System.Random rand = new System.Random();

        // Initialise possibilities
        var allTileOptions = neighbourData.Keys.ToList();
        Debug.Log("All Tile Options: " + String.Join(", ", allTileOptions));

        var map = new MapSquareOptions[mapHeight * mapWidth];
        for(int y = 0; y < mapHeight; y++)
            for(int x = 0; x < mapWidth; x++)
                map[y * mapWidth + x] = new MapSquareOptions(x, y, allTileOptions);

        var mapCenter = new Vector2(mapWidth / 2, mapHeight / 2);

        while(map.Any(ms => !ms.IsCollapsed))
        {
            // Select square that isn't collapsed yet with lowest possibilities
            var target = map.Where(ms => !ms.IsCollapsed).OrderBy(ms => ms.NumberOfPossibilities).ThenBy(ms => (new Vector2(ms.X, ms.Y) - mapCenter).magnitude).First();

            // Choose random tile
            if(target.Options.Contains("BasicFloor") && rand.NextDouble() <= 0.9f)
                target.Choose("BasicFloor");
            else if(target.Options.Contains("BasicRaisedFloor") && rand.NextDouble() <= 0.9f)
                target.Choose("BasicRaisedFloor");
            else
                target.Choose(target.Options[rand.Next(target.Options.Count)]);
            //Debug.Log($"Square X{target.X} Y{target.Y} given {target.TileName}");

            // Collapse outward - recurse until stable
            CollapseAt(target, map);
        }

        var tiles = map.Select(m => meshData[m.TileName]).ToArray();

        return tiles;
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
