#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CIMenu
{
    [MenuItem("Smolbean/Build/Build All (local)")]
    public static void BuildAll()
    {
        new CIBuilderWindows().Run();
        new CIBuilderLinux().Run();
        new CIBuilderMac().Run();
        Debug.Log("Smolbean: Build All (local) finished.");
    }

    [MenuItem("Smolbean/Build/Build Windows")]
    public static void BuildWindows() => new CIBuilderWindows().Run();

    [MenuItem("Smolbean/Build/Build Linux")]
    public static void BuildLinux() => new CIBuilderLinux().Run();

    [MenuItem("Smolbean/Build/Build macOS")]
    public static void BuildMac() => new CIBuilderMac().Run();
}
#endif
