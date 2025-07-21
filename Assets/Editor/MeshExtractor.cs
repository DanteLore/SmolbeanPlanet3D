#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

public static class MeshExtractor
{
    private const string OUTPUT_FOLDER = "Assets/Meshes/AutoExtractedFromScene";

    [MenuItem("Smolbean/Extract scene meshes to decrease .unity file size")]
    public static void ExtractAllMeshes()
    {
        // Ensure output folder exists
        if (!Directory.Exists(OUTPUT_FOLDER))
            Directory.CreateDirectory(OUTPUT_FOLDER);

        var scene = SceneManager.GetActiveScene();
        Debug.Log($"Starting mesh extraction in scene '{scene.name}'...");

        foreach (var root in scene.GetRootGameObjects())
        {
            ProcessGameObject(root);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.MarkSceneDirty(scene);

        Debug.Log("✅ Mesh extraction complete! Check " + OUTPUT_FOLDER);
    }

    private static void ProcessGameObject(GameObject go)
    {
        Debug.Log($"Processing GameObject: {go.name}");

        // Iterate all components on this GameObject
        foreach (var comp in go.GetComponents<Component>())
        {
            if (comp == null) continue;  // missing script stub

            var so = new SerializedObject(comp);
            var iter = so.GetIterator();

            while (iter.NextVisible(true))
            {
                if (iter.propertyType == SerializedPropertyType.ObjectReference)
                {
                    var mesh = iter.objectReferenceValue as Mesh;
                    if (mesh != null)
                    {
                        var path = AssetDatabase.GetAssetPath(mesh);
                        if (string.IsNullOrEmpty(path))
                        {
                            Debug.Log($"  Found embedded mesh in component '{comp.GetType().Name}' on '{go.name}'");
                            var newMesh = ExtractMeshAsset(mesh, go.name, comp.GetType().Name);
                            iter.objectReferenceValue = newMesh;
                            so.ApplyModifiedProperties();
                            Debug.Log($"    Extracted to {AssetDatabase.GetAssetPath(newMesh)}");
                        }
                    }
                }
            }
        }

        // Recurse into children
        foreach (Transform child in go.transform)
        {
            ProcessGameObject(child.gameObject);
        }
    }

    private static Mesh ExtractMeshAsset(Mesh originalMesh, string goName, string compType)
    {
        // Duplicate the mesh data
        var meshCopy = Object.Instantiate(originalMesh);

        // Generate a safe filename using GameObject name and component type
        string assetPath = Path.Combine(OUTPUT_FOLDER, $"{goName}_{compType}.asset");
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(meshCopy, assetPath);
        AssetDatabase.ImportAsset(assetPath);

        var extracted = AssetDatabase.LoadAssetAtPath<Mesh>(assetPath);
        EditorUtility.SetDirty(meshCopy);
        return extracted;
    }
}
#endif
