using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var gridManager = (GridManager)target;
        gridManager.BootstrapMapData();

        if(GUILayout.Button("Wave Function Collapse"))
        {
            var generator = GameObject.FindAnyObjectByType<GameMapGenerator>();

            var newMap = generator.GenerateMap(generator.seed);
            gridManager.mapData.GameMap = newMap.ToArray();
            gridManager.mapData.GameMapWidth = generator.mapWidth;
            gridManager.mapData.GameMapHeight = generator.mapHeight;
            EditorUtility.SetDirty(gridManager.mapData);
            AssetDatabase.SaveAssetIfDirty(gridManager.mapData);

            gridManager.Recreate(newMap, generator.mapWidth, generator.mapHeight);
            EditorUtility.SetDirty(target);
        }
    }
}