using UnityEditor;
using UnityEngine;
using System.Linq;

[CustomEditor(typeof(DropController))]
public class DropControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Assign All DropSpecs"))
        {
            var dropController = (DropController)target;
            
            var specs = AssetDatabase.FindAssets($"t: {typeof(DropSpec).Name}").ToList()
                                        .Select(AssetDatabase.GUIDToAssetPath)
                                        .Select(AssetDatabase.LoadAssetAtPath<DropSpec>)
                                        .ToArray();
            dropController.dropSpecs = specs;

            EditorUtility.SetDirty(target);
        }
    }
}
