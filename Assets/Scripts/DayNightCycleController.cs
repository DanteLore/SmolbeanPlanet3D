using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycleController : MonoBehaviour, IObjectGenerator
{
    public Gradient ambientLightColor;
    [GradientUsage(hdr:true)] public Gradient sunColor;
    public Gradient directionalLight;
    public Gradient fog;
    public AnimationCurve seasonCurve;
    public Light sunLight;
    public Material skyboxMaterial;
    public float hourLengthSeconds = 12f;

    [Range(0f, 24f)]
    public float gameStartTime = 7f;

    [Range(0f, 24f)]
    public float timeOfDay;

    public int Priority { get { return 1; } }

    public void Clear()
    {
        Debug.Log("DayNightCycleController.Generate");
        timeOfDay = gameStartTime;
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        Debug.Log("DayNightCycleController.Generate");
        timeOfDay = gameStartTime;
    }

    public TimeOfDaySaveData GetSaveData()
    {
        return new TimeOfDaySaveData { timeOfDay = timeOfDay };
    }

    public void LoadState(TimeOfDaySaveData loadedData)
    {
        timeOfDay = loadedData.timeOfDay;
    }

    void Update()
    {
        if(Application.isPlaying)
        {
            timeOfDay += Time.deltaTime / hourLengthSeconds;
            timeOfDay %= 24f;
        }

        float tod = seasonCurve.Evaluate(timeOfDay / 24f);
        float angle = 360f * tod - 90f;

        RenderSettings.ambientLight = ambientLightColor.Evaluate(tod);
        RenderSettings.fogColor = fog.Evaluate(tod);

        transform.rotation = Quaternion.Euler(0f, -90f, 30f);

        sunLight.color = directionalLight.Evaluate(tod);
        sunLight.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);

        skyboxMaterial.SetVector("_sunDirection", sunLight.transform.forward);
        skyboxMaterial.SetVector("_sunColor", sunColor.Evaluate(tod));
    }
}
