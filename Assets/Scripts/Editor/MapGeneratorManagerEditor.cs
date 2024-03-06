using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGeneratorManager))]
public class MapGeneratorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Map"))
        {
            EditorCoroutineUtility.StartCoroutine(DrawMap(), this);
            
            EditorUtility.SetDirty(target);
        }
    }

    private IEnumerator DrawMap()
    {
        var mapGenerator = (MapGeneratorManager)target;
        mapGenerator.BootstrapMapData();

        var generator = FindAnyObjectByType<GameMapCreator>();

        var newMap = generator.GenerateMap(generator.seed);
        mapGenerator.mapData.GameMap = newMap.ToArray();
        mapGenerator.mapData.GameMapWidth = generator.mapWidth;
        mapGenerator.mapData.GameMapHeight = generator.mapHeight;
        mapGenerator.mapData.MaxLevelNumber = generator.maxLevelNumber;
        EditorUtility.SetDirty(mapGenerator.mapData);
        AssetDatabase.SaveAssetIfDirty(mapGenerator.mapData);

        yield return mapGenerator.Recreate(newMap, generator.mapWidth, generator.mapHeight);
    }
}