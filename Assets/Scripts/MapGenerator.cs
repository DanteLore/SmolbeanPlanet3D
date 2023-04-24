using System.Linq;
using System.Collections.Generic;

public class MapGenerator
{
    public class MapSquareOptions
    {
        List<string> options;
    }

    private MapSquareOptions[] map;
    private int mapWidth = 10;
    private int mapHeight = 10;
    private List<MeshData> meshData;

    public MapGenerator(int mapWidth, int mapHeight, List<MeshData> meshData)
    {
        this.mapWidth = mapWidth;
        this.mapHeight = mapHeight;
        this.meshData = meshData;

        this.map = new MapSquareOptions[this.mapWidth * this.mapHeight];
    }

    public MeshData[] GenerateMap()
    {
        var tiles = new MeshData[mapHeight * mapWidth];

        var md = meshData.First(md => md.name.ToLower().Contains("bumpyfloor"));

        for(int y = 0; y < mapWidth; y++)
            for(int x = 0; x < mapWidth; x++)
                tiles[y * mapWidth + x] = md;

        return tiles;
    }

}
