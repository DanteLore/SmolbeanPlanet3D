#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;

public static class BuilderRegistry
{
    // key -> concrete builder type (case-insensitive)
    private static readonly Dictionary<string, Type> _map = new(StringComparer.OrdinalIgnoreCase);
    private static bool _initialized;

    public static void EnsureInitialized()
    {
        if (_initialized) return;
        _initialized = true;

        // Find all non-abstract classes deriving BuildTaskBase
        foreach (var t in TypeCache.GetTypesDerivedFrom<BuildTaskBase>())
        {
            if (t.IsAbstract) continue;

            // If it has BuildKey attributes, register them
            var attrs = (BuildKeyAttribute[])Attribute.GetCustomAttributes(t, typeof(BuildKeyAttribute), false);
            if (attrs != null && attrs.Length > 0)
            {
                foreach (var a in attrs)
                    SafeAdd(a.Key, t);
            }

            // Also register by BuildTarget enum name (e.g., StandaloneWindows64)
            try
            {
                var instance = (BuildTaskBase)Activator.CreateInstance(t);
                SafeAdd(instance.Target.ToString(), t);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"BuilderRegistry: could not instantiate {t.Name}: {e.Message}");
            }
        }
    }

    private static void SafeAdd(string key, Type t)
    {
        if (string.IsNullOrWhiteSpace(key)) return;
        if (_map.ContainsKey(key)) return; // first writer wins; avoid alias collisions
        _map[key] = t;
    }

    public static BuildTaskBase Resolve(string key)
    {
        EnsureInitialized();
        if (!_map.TryGetValue(key, out var type))
            throw new ArgumentException($"Unknown build key '{key}'. Known keys: {string.Join(", ", _map.Keys)}");
        return (BuildTaskBase)Activator.CreateInstance(type);
    }
}
#endif
