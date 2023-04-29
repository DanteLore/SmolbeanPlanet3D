using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaneMeshGenerator))]
public class PlaneMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlaneMeshGenerator generator = (PlaneMeshGenerator)target;
        if (GUILayout.Button("Generate Plane"))
        {
            generator.GeneratePlane();
        }
    }
}
