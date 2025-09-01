#if UNITY_EDITOR
using UnityEditor;

[BuildKey("mac")]
[BuildKey("osx")]
[BuildKey("StandaloneOSX")]
public sealed class CIBuilderMac : BuildTaskBase
{
    public override BuildTarget Target => BuildTarget.StandaloneOSX;
    public override string DefaultOutputPath => "Builds/macOS/SmolbeanPlanet.app";
}
#endif
