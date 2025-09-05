#if UNITY_EDITOR
using System;
using UnityEngine;

public static class CIBuildEntryCLI
{
    // Examples:
    // -executeMethod CIBuildEntryCLI.BuildCI -buildKey mac -outputPath Builds/macOS/SmolbeanPlanet.app -development false -buildProfile test -requireProfile true
    // -executeMethod CIBuildEntryCLI.BuildCI -buildTarget StandaloneWindows64 -outputPath Builds/Windows/SmolbeanPlanet/SmolbeanPlanet.exe -development false -buildProfile test
    // -executeMethod CIBuildEntryCLI.ListBuilders

    public static void BuildCI()
    {
        // Accept either -buildKey (preferred) or -buildTarget (fallback).
        var buildKey       = GetArg("-buildKey",   "");
        var buildTargetKey = GetArg("-buildTarget",""); // e.g., StandaloneOSX
        var key            = !string.IsNullOrEmpty(buildKey) ? buildKey
                          : !string.IsNullOrEmpty(buildTargetKey) ? buildTargetKey
                          : "StandaloneLinux64"; // default

        var outPath         = GetArg("-outputPath", "");
        bool dev            = GetArg("-development", "false").ToLowerInvariant() == "true";
        var profileName     = GetArg("-buildProfile", "");
        bool requireProfile = GetArg("-requireProfile", "false").ToLowerInvariant() == "true";

        // Activate Build Profile first (Unity 6; safe no-op otherwise).
        if (!string.IsNullOrEmpty(profileName))
        {
            var ok = BuildProfileActivator.TryActivate(profileName);
            if (!ok)
            {
                var msg = $"[CI] Build Profile '{profileName}' not activated.";
                if (requireProfile) throw new Exception(msg);
                Debug.LogWarning(msg + " Proceeding without it.");
            }
        }

        // Resolve the concrete builder via your registry.
        var builder = BuilderRegistry.Resolve(key);

        // Run the build.
        builder.Run(dev, string.IsNullOrEmpty(outPath) ? null : outPath);
    }

    public static void ListBuilders()
    {
        // Convenience to see what's registered on the CI box
        BuilderRegistry.EnsureInitialized();
        Debug.Log("Available build keys:\n" +
                  "(Use your own method to enumerate keys if you have one; " +
                  "currently BuilderRegistry keeps keys internal. Consider adding a Keys() accessor.)");
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
