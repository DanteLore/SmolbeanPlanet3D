#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

internal static class BuildProfileActivator
{
    public static bool TryActivate(string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName)) return false;
        try
        {
            var typeNames = new[]
            {
                "UnityEditor.Build.Profile.BuildProfile",
                "UnityEditor.Build.BuildProfile",
                "UnityEditor.BuildProfile"
            };
            Type t = null;
            foreach (var tn in typeNames)
            {
                t = Type.GetType(tn + ", UnityEditor", throwOnError: false);
                if (t != null) break;
            }
            if (t == null)
            {
                Debug.LogWarning($"[CI] Build Profile API not found on {Application.unityVersion}. Skipping activation.");
                return false;
            }

            var getProfile = t.GetMethod("GetProfile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(string) }, null);
            var setActive  = t.GetMethod("SetActiveProfile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (getProfile == null || setActive == null)
            {
                Debug.LogWarning("[CI] BuildProfile.GetProfile/SetActiveProfile not found. Skipping activation.");
                return false;
            }

            var profileObj = getProfile.Invoke(null, new object[] { profileName });
            if (profileObj == null)
            {
                Debug.LogError($"[CI] Build profile '{profileName}' not found.");
                return false;
            }

            setActive.Invoke(null, new[] { profileObj });

            var getActive = t.GetMethod("GetActiveProfile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            string activeName = profileName;
            if (getActive != null)
            {
                var activeObj = getActive.Invoke(null, null);
                var nameProp = activeObj?.GetType().GetProperty("name", BindingFlags.Public | BindingFlags.Instance);
                activeName = nameProp?.GetValue(activeObj) as string ?? activeName;
            }
            Debug.Log($"[CI] Activated Build Profile: {activeName}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[CI] Failed to activate Build Profile: {ex.Message}");
            return false;
        }
    }
}
#endif
