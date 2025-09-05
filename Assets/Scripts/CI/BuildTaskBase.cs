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
    
    protected static string[] DefaultScenes => Array.ConvertAll(EditorBuildSettings.scenes, s => s.path);

    public void Run(string outputPathOverride = null)
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
            scenes = DefaultScenes,
            target = Target,
            locationPathName = outputPath,
            options = BuildOptions.None
        };

        var namedTarget = NamedBuildTarget.FromBuildTargetGroup(group);

        PlayerSettings.SetScriptingBackend(namedTarget, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApiCompatibilityLevel(namedTarget, ApiCompatibilityLevel.NET_Unity_4_8);

        PrintDebugSnapshotInfo(namedTarget);

        var report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception($"Build failed: {report.summary.result} ({report.summary.totalErrors} errors).");

        CleanupDoNotShip(OutputDirOf(outputPath));

        Debug.Log($"✅ Build OK: {Target} → {outputPath}");
    }

    private void PrintDebugSnapshotInfo(NamedBuildTarget namedTarget)
    {
        var backend = PlayerSettings.GetScriptingBackend(namedTarget);
        var api = PlayerSettings.GetApiCompatibilityLevel(namedTarget);
        var defines = PlayerSettings.GetScriptingDefineSymbols(namedTarget);

        Debug.Log($"[CI] Snapshot → Strip={PlayerSettings.GetManagedStrippingLevel(namedTarget)}, " +
                $"Backend={backend}, APICompat={api}, Defines={defines}, " +
                $"Graphics={string.Join(";", PlayerSettings.GetGraphicsAPIs(Target))}, " +
                $"UnityVersion={Application.unityVersion}");
    }

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
        foreach (var d in Directory.GetDirectories(buildDir, "*", SearchOption.AllDirectories))
            if (d.IndexOf("donotship", StringComparison.OrdinalIgnoreCase) >= 0)
                Directory.Delete(d, true);
    }
}
#endif
