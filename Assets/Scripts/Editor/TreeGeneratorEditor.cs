using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TreeGenerator))]
public class TreeGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("GenerateTrees"))
        {
            var treeGenerator = (TreeGenerator)target;
            treeGenerator.GenerateTrees();
        }
    }
}