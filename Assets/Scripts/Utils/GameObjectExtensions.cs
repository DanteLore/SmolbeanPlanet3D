using System.Linq;
using UnityEngine;

public static class GameObjectExtensions
{
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        if (parent.name == name)
            return parent;

        foreach (Transform child in parent)
        {
            Transform found = child.FindDeepChild(name);
            if (found != null)
                return found;
        }

        return null;
    }

    public static Bounds GetRendererBounds(this GameObject go, bool activeOnly = true)
    {
        return go.transform.GetRendererBounds(activeOnly: activeOnly);
    }

    public static Bounds GetRendererBounds(this Transform transform, bool activeOnly = true)
    {
        var allBounds = transform
            .GetComponentsInChildren<Renderer>()
            .Where(c => c.gameObject.activeInHierarchy || !activeOnly)
            .Where(c => c.GetComponent<ParticleSystem>() == null)
            .Select(r => r.bounds)
            .ToArray();

        if (allBounds.Length == 0)
            return new Bounds(transform.position, Vector3.zero);

        var bounds = allBounds[0];
        for (int i = 1; i < allBounds.Length; i++)
            bounds.Encapsulate(allBounds[i]);

        return bounds;
    }
}
