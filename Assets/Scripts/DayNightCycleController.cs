using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycleController : MonoBehaviour, IObjectGenerator
{
    public static DayNightCycleController Instance { get; private set; }
    public bool NewGameOnly { get { return false; } }
    public bool RunModeOnly { get { return true; } }
    public Gradient ambientLightColor;
    [GradientUsage(hdr:true)] public Gradient sunColor;
    public Gradient directionalLight;
    public Gradient fog;
    public AnimationCurve seasonCurve;
    public Light sunLight;
    public Material[] sunSensitiveMaterials;
    public float hourLengthSeconds = 12f;

    [Range(0f, 24f)]
    public float gameStartTime = 7f;

    [Range(0f, 24f)]
    public float timeOfDay;

    public int day = 1;

    public int Priority { get { return 1; } }

    public string DisplayTime
    {
        get
        {
            int hour = Mathf.FloorToInt(timeOfDay);
            float d = timeOfDay % 1f;

            if(d <= 0.25)
                return $"🜚 {hour}";
            if(d <= 0.50)
                return $"🜚 {hour}.¼";
            if(d <= 0.75)
                return $"🜚 {hour}.½";
            else
                return $"🜚 {hour}.¾";
        }
    }

    public string DisplayDay
    {
        get
        {
            return $"🜳 {day}";
        }
    }

    public string DisplayTimeAndDay
    {
        get
        {
            return $"{DisplayDay}   {DisplayTime}";
        }
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Clear()
    {
        timeOfDay = gameStartTime;
        day = 1;
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        timeOfDay = gameStartTime;
        day = 1;
    }

    public TimeOfDaySaveData GetSaveData()
    {
        return new TimeOfDaySaveData { timeOfDay = timeOfDay, day = day };
    }

    public void LoadState(TimeOfDaySaveData loadedData)
    {
        timeOfDay = loadedData.timeOfDay;
        day = loadedData.day;
    }

    void Update()
    {
        if(Application.isPlaying)
        {
            timeOfDay += Time.deltaTime / hourLengthSeconds;
            
            if(timeOfDay > 24f)
            {
                day++;
                timeOfDay %= 24f;
            }
        }

        float tod = seasonCurve.Evaluate(timeOfDay / 24f);
        float angle = 360f * tod - 90f;

        var ambientLight = ambientLightColor.Evaluate(tod);
        RenderSettings.ambientLight = ambientLight;
        RenderSettings.fogColor = fog.Evaluate(tod);

        transform.rotation = Quaternion.Euler(0f, -90f, 30f);

        var directionalLightColor = directionalLight.Evaluate(tod);
        sunLight.color = directionalLightColor;
        sunLight.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);

        foreach(var mat in sunSensitiveMaterials)
        {
            mat.SetVector("_sunDirection", sunLight.transform.forward);
            mat.SetVector("_sunColor", sunColor.Evaluate(tod));
            mat.SetVector("_ambientLightColor", ambientLight);
            mat.SetVector("_directionalLightColor", ambientLight);
        }
    }
}
