using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Smolbean/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public int[] GameMap;

    public void SetGameMap(int[] value)
    {
        GameMap = value;
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
#endif
    }
}
