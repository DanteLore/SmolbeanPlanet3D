using System.Collections;
using System.Collections.Generic;

public interface IObjectGenerator
{
    public int Priority { get; }
    public bool RunModeOnly { get; }
    public void Clear();
    public void SaveTo(SaveFileData saveData, string filename);

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight);
    public IEnumerator Load(SaveFileData data, string filename);
}
