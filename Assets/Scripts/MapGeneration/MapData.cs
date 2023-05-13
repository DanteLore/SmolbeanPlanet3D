using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Smolbean/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public int[] GameMap;
    public int GameMapWidth;
    public int GameMapHeight;
}
