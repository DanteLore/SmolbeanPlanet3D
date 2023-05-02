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
            var gridManager = (GridManager)target;
            gridManager.Recreate();
        }
    }
}