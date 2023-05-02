using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameMapGenerator))]
public class GameMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameMapGenerator generator = (GameMapGenerator)target;
        if (GUILayout.Button("Generate Game Map"))
        {
            generator.GenerateMap();
        }
        if (GUILayout.Button("Hide Map Preview"))
        {
            generator.HidePreview();
        }
    }
}
