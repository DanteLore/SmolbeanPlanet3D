#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public abstract class BuildTaskBase
{
    public abstract BuildTarget Target { get; }
    public abstract string DefaultOutputPath { get; }

    public void Run(bool development, string outputPathOverride = null)
    {
        var group = BuildPipeline.GetBuildTargetGroup(Target);
        if (!BuildPipeline.IsBuildTargetSupported(group, Target))
        {
            Debug.LogWarning($"Skipping {Target}: module not installed (Unity Hub → Add modules).");
            return;
        }

        string outputPath = string.IsNullOrEmpty(outputPathOverride) ? DefaultOutputPath : outputPathOverride;
        EnsureOutputDir(outputPath);

        var opts = new BuildPlayerOptions
        {
            scenes = DefaultScenes(),
            target = Target,
            locationPathName = outputPath,
            options = development ? BuildOptions.Development | BuildOptions.AllowDebugging : BuildOptions.None
        };

        var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);

        // enforce settings
        PlayerSettings.SetScriptingBackend(namedTarget, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApiCompatibilityLevel(namedTarget, ApiCompatibilityLevel.NET_Unity_4_8);

        // logging
        var backend = PlayerSettings.GetScriptingBackend(namedTarget);
        var api     = PlayerSettings.GetApiCompatibilityLevel(namedTarget);
        var defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);

        Debug.Log($"[CI] Snapshot → Dev={development}, Strip={PlayerSettings.GetManagedStrippingLevel(namedTarget)}, " +
                $"Backend={backend}, APICompat={api}, Defines={defines}, " +
                $"Graphics={string.Join(";", PlayerSettings.GetGraphicsAPIs(Target))}, " +
                $"UnityVersion={Application.unityVersion}");

        var report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception($"Build failed: {report.summary.result} ({report.summary.totalErrors} errors).");

        CleanupDoNotShip(OutputDirOf(outputPath));

        Debug.Log($"✅ Build OK: {Target} → {outputPath}");
    }

    // ---- Shared helpers ----
    protected static string[] DefaultScenes() =>
        Array.ConvertAll(EditorBuildSettings.scenes, s => s.path);

    protected static void EnsureOutputDir(string outputPath)
    {
        var dir = OutputDirOf(outputPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    protected static string OutputDirOf(string outputPath)
    {
        bool hasExt = !string.IsNullOrEmpty(Path.GetExtension(outputPath)) ||
                      outputPath.EndsWith(".app", StringComparison.OrdinalIgnoreCase);
        return hasExt ? Path.GetDirectoryName(outputPath) : outputPath;
    }

    protected static void CleanupDoNotShip(string buildDir)
    {
        if (string.IsNullOrEmpty(buildDir) || !Directory.Exists(buildDir)) return;

        foreach (var d in Directory.GetDirectories(buildDir, "*", SearchOption.AllDirectories))
            if (d.IndexOf("donotship", StringComparison.OrdinalIgnoreCase) >= 0)
                TryDeleteDir(d);

        foreach (var f in Directory.GetFiles(buildDir, "*", SearchOption.AllDirectories))
            if (f.IndexOf("donotship", StringComparison.OrdinalIgnoreCase) >= 0)
                TryDeleteFile(f);

        Debug.Log("🧹 Cleaned DoNotShip artifacts.");
    }

    protected static void TryDeleteDir(string path)
    {
        try { Directory.Delete(path, true); }
        catch (Exception e) { Debug.LogWarning($"Could not delete dir '{path}': {e.Message}"); }
    }

    protected static void TryDeleteFile(string path)
    {
        try { File.Delete(path); }
        catch (Exception e) { Debug.LogWarning($"Could not delete file '{path}': {e.Message}"); }
    }
}
#endif
