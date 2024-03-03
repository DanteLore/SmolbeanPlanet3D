using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Wave Function Collapse"))
        {
            EditorCoroutineUtility.StartCoroutine(DrawMap(), this);
            
            EditorUtility.SetDirty(target);
        }
    }

    private IEnumerator DrawMap()
    {
        var gridManager = (GridManager)target;
        gridManager.BootstrapMapData();

        var generator = FindAnyObjectByType<GameMapGenerator>();

        var newMap = generator.GenerateMap(generator.seed);
        gridManager.mapData.GameMap = newMap.ToArray();
        gridManager.mapData.GameMapWidth = generator.mapWidth;
        gridManager.mapData.GameMapHeight = generator.mapHeight;
        gridManager.mapData.MaxLevelNumber = generator.maxLevelNumber;
        EditorUtility.SetDirty(gridManager.mapData);
        AssetDatabase.SaveAssetIfDirty(gridManager.mapData);

        yield return gridManager.Recreate(newMap, generator.mapWidth, generator.mapHeight, true);
    }
}