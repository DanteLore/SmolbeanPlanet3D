using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycleController : MonoBehaviour
{
    public Gradient ambientLightColor;
    public Gradient directionalLight;
    public Gradient fog;
    public AnimationCurve seasonCurve;
    public Light sunLight;

    [Range(0f, 24f)]
    public float timeOfDay;

    void Update()
    {
        if(Application.isPlaying)
        {
            timeOfDay += Time.deltaTime;
            timeOfDay %= 24f;
        }

        float tod = seasonCurve.Evaluate(timeOfDay / 24f);
        float angle = 360f * tod - 90f;

        RenderSettings.ambientLight = ambientLightColor.Evaluate(tod);
        RenderSettings.fogColor = fog.Evaluate(tod);
        sunLight.color = directionalLight.Evaluate(tod);
        sunLight.transform.rotation = Quaternion.Euler(angle, -90f, 0f);
    }
}
