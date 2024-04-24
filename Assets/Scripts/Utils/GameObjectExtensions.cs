using System.Linq;
using UnityEngine;

public static class GameObjectExtensions
{
    public static Bounds GetRendererBounds(this GameObject go)
    {
        return go.transform.GetRendererBounds();
    }

    public static Bounds GetRendererBounds(this Transform transform)
    {
        var allBounds = transform
            .GetComponentsInChildren<Renderer>()
            .Where(c => c.gameObject.activeInHierarchy)
            .Where(c => c.GetComponent<ParticleSystem>() == null)
            .Select(r => r.bounds)
            .ToArray();

        var bounds = allBounds[0];

        for (int i = 1; i < allBounds.Length; i++)
            bounds.Encapsulate(allBounds[i]);

        return bounds;
    }
}
