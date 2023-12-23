using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameMapGenerator))]
public class GameMapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var gameMapGenerator = (GameMapGenerator)target;

        if(GUILayout.Button("Generate Map"))
        {
            Debug.Log("Hello!");
        }
    }
}