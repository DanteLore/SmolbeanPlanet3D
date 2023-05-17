using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Trees"))
        {
            var treeGenerator = (TreeGenerator)target;
            treeGenerator.Generate(treeGenerator.mapData.GameMap.ToList(), treeGenerator.mapData.GameMapWidth, treeGenerator.mapData.GameMapHeight);
            EditorUtility.SetDirty(target);
        }
    }
}