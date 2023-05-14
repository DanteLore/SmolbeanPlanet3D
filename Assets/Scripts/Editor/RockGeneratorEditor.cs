using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(RockGenerator))]
public class RockGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Rocks"))
        {
            var rockGenerator = (RockGenerator)target;
            rockGenerator.GenerateRocks(rockGenerator.mapData.GameMap.ToList(), rockGenerator.mapData.GameMapWidth, rockGenerator.mapData.GameMapHeight);
            EditorUtility.SetDirty(target);
        }
    }
}