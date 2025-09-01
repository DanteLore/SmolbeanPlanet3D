#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class CIMenu
{
    [MenuItem("Smolbean/Build/Build All (local)")]
    public static void BuildAll()
    {
        new CIBuilderWindows().Run(false);
        new CIBuilderLinux().Run(false);
        new CIBuilderMac().Run(false);
        Debug.Log("Smolbean: Build All (local) finished.");
    }

    [MenuItem("Smolbean/Build/Build Windows")]
    public static void BuildWindows() => new CIBuilderWindows().Run(false);

    [MenuItem("Smolbean/Build/Build Linux")]
    public static void BuildLinux() => new CIBuilderLinux().Run(false);

    [MenuItem("Smolbean/Build/Build macOS")]
    public static void BuildMac() => new CIBuilderMac().Run(false);
}
#endif
