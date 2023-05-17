using System.Collections.Generic;

public interface IObjectGenerator
{
    public int Priority { get; }
    public void Clear();

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight);
}
