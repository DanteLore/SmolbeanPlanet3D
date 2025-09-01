#if UNITY_EDITOR
using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class BuildKeyAttribute : Attribute
{
    public string Key { get; }
    public BuildKeyAttribute(string key) => Key = key;
}
#endif
