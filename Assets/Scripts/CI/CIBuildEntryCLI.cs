#if UNITY_EDITOR
using System;
using UnityEditor;
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
        var profileName = GetArg("-buildProfile", "Smolbean");

        SetBuildProfile(profileName);

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

    private static void SetBuildProfile(string profileName)
    {
        Debug.Log($"[CI] Current build profile: {BuildProfile.GetActiveBuildProfile().name}");
        var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>($"Assets/Settings/Build Profiles/{profileName}.asset");
        if (profile == null)
            throw new Exception($"[CI] Build Profile not found: {profileName}");
        BuildProfile.SetActiveBuildProfile(profile);
        Debug.Log($"[CI] Switched to build profile: {BuildProfile.GetActiveBuildProfile().name}");
    }
}
#endif
