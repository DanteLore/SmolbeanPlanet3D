using System.Linq;
using UnityEngine;

public static class GameObjectExtensions
{
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

        var bounds = allBounds[0];
        for (int i = 1; i < allBounds.Length; i++)
            bounds.Encapsulate(allBounds[i]);

        return bounds;
    }
}
