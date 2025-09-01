#if UNITY_EDITOR
using System;

public static class CIBuildEntryCLI
{
    // Unity CLI example:
    // -executeMethod CIBuildEntry.BuildCI \
    //   -buildTarget StandaloneLinux64 \
    //   -outputPath Builds/Linux/SmolbeanPlanet.x86_64 \
    //   -development false
    public static void BuildCI()
    {
        var target  = GetArg("-buildTarget", "StandaloneLinux64");
        var outPath = GetArg("-outputPath", "");
        bool dev    = GetArg("-development", "false").ToLowerInvariant() == "true";

        BuildTaskBase builder = target switch
        {
            "StandaloneWindows64" => new CIBuilderWindows(),
            "StandaloneLinux64"   => new CIBuilderLinux(),
            "StandaloneOSX"       => new CIBuilderMac(),
            _ => throw new ArgumentException($"Unknown build target '{target}'")
        };

        builder.Run(dev, string.IsNullOrEmpty(outPath) ? null : outPath);
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
