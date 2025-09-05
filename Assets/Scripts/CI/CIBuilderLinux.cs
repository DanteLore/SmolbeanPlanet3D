#if UNITY_EDITOR
using UnityEditor;

[BuildKey("linux")]
[BuildKey("StandaloneLinux64")]
public sealed class CIBuilderLinux : BuildTaskBase
{
    public override BuildTarget Target => BuildTarget.StandaloneLinux64;
    public override string DefaultOutputPath => "Builds/Linux/SmolbeanPlanet.x86_64";
}
#endif
