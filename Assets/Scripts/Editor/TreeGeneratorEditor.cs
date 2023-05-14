using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("GenerateTrees"))
        {
            var treeGenerator = (TreeGenerator)target;
            treeGenerator.GenerateTrees(treeGenerator.mapData.GameMap.ToList(), treeGenerator.mapData.GameMapWidth, treeGenerator.mapData.GameMapHeight);
            EditorUtility.SetDirty(target);
        }
    }
}