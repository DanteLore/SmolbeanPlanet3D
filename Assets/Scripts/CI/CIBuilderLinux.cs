#if UNITY_EDITOR
using UnityEditor;

public sealed class CIBuilderLinux : BuildTaskBase
{
    public override BuildTarget Target => BuildTarget.StandaloneLinux64;
    public override string DefaultOutputPath => "Builds/Linux/SmolbeanPlanet.x86_64";
}
#endif
