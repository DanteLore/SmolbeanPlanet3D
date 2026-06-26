using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ManaFlyer : MonoBehaviour
{
    public Vector3 startPoint;
    public float durationSeconds = 10f;
    public Camera mainCamera;
    public float targetZDistance = 1.0f;
    [NonSerialized] public VisualElement uiElement;

    private float elapsed;

    private void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= durationSeconds)
            Destroy(gameObject);

        var bounds = uiElement.worldBound;               
        Vector2 uiCenter = bounds.center;

        Vector3 screenPoint = new(
            uiCenter.x,
            Screen.height - uiCenter.y,
            targetZDistance
        );

        var endPoint = mainCamera.ScreenToWorldPoint(screenPoint);

        float t = Mathf.Clamp01(elapsed / durationSeconds);
        transform.position = Vector3.Lerp(startPoint, endPoint, t);
    }
}
