#if UNITY_EDITOR
using UnityEditor;

[BuildKey("win64")]
[BuildKey("windows")]
[BuildKey("StandaloneWindows64")] 
public sealed class CIBuilderWindows : BuildTaskBase
{
    public override BuildTarget Target => BuildTarget.StandaloneWindows64;
    public override string DefaultOutputPath => "Builds/Windows/SmolbeanPlanet.exe";
}
#endif
