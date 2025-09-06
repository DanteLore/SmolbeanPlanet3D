#if UNITY_EDITOR
using System;
using UnityEditor.Build.Profile;
using UnityEngine;

public static class CIBuildEntryCLI
{
    public static void BuildCI()
    {
        // Accept either -buildKey (preferred) or -buildTarget (fallback).
        var buildKey = GetArg("-buildKey", "");
        var buildTargetKey = GetArg("-buildTarget", ""); // e.g., StandaloneOSX
        var key = !string.IsNullOrEmpty(buildKey) ? buildKey
                          : !string.IsNullOrEmpty(buildTargetKey) ? buildTargetKey
                          : "StandaloneLinux64"; // default

        var outPath = GetArg("-outputPath", "");

        var buildProfile = BuildProfile.GetActiveBuildProfile();
        var buildProfileName = buildProfile != null ? buildProfile.name : "null";
        Debug.Log($"[CI] Current build profile: {buildProfileName}");

        // Resolve the concrete builder via your registry.
        var builder = BuilderRegistry.Resolve(key);

        // Run the build.
        builder.Run(string.IsNullOrEmpty(outPath) ? null : outPath);
    }

    private static string GetArg(string name, string fallback)
    {
        var args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
            if (args[i] == name && i + 1 < args.Length) return args[i + 1];
        return fallback;
    }
}
#endif
