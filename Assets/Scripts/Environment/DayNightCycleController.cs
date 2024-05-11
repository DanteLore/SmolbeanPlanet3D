using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycleController : MonoBehaviour, IObjectGenerator
{
    public static DayNightCycleController Instance { get; private set; }
    public bool RunModeOnly { get { return true; } }
    public int Priority { get { return 4; } }

    [SerializeField] private Gradient ambientLightColor;
    [SerializeField] [GradientUsage(hdr:true)] private Gradient sunColor;
    [SerializeField] private Gradient directionalLight;
    [SerializeField] private Gradient fog;
    [SerializeField] private AnimationCurve seasonCurve;
    [SerializeField] private Light sunLight;
    [SerializeField] private Material[] sunSensitiveMaterials;
    [SerializeField] private float hourLengthSeconds = 12f;
    [SerializeField] private float lightLevel;
    [SerializeField] [Range(0f, 24f)] private float gameStartTime = 7f;
    [SerializeField] [Range(0f, 24f)] private float timeOfDay;
    [SerializeField] private int day = 1;
    [SerializeField] [Range(-90, 90)] private float sunAngleOffset = 0f;
    [SerializeField] private bool forceTimePause = false;

    public string DisplayTimeAndDay { get => $"{DisplayDay()}   {DisplayTime()}"; }
    public int Day { get => day; }
    public float TimeOfDay { get => timeOfDay; }
    public float LightLevel { get => lightLevel; }
    public float HourLengthSeconds { get => hourLengthSeconds; }

    public bool TimeIsBetween(float start, float end)
    {
        if (start < end)
            return start <= TimeOfDay && TimeOfDay <= end;
        else
            return TimeOfDay <= end || TimeOfDay >= start;
    }

    public string DisplayTime(float t = -1f)
    {
        float tod = t == -1 ? TimeOfDay : t;

        int hour = Mathf.FloorToInt(tod);
        float d = tod % 1f;

        if (d <= 0.25)
            return $"🜚 {hour}";
        if (d <= 0.50)
            return $"🜚 {hour}.¼";
        if (d <= 0.75)
            return $"🜚 {hour}.½";
        else
            return $"🜚 {hour}.¾";
    }

    public string DisplayDay(int d = -1)
    {
        int x = d == -1 ? Day : d;
        return $"🜳 {x}";
    }

    public string DurationToString(float d)
    {
        int days = Mathf.FloorToInt(d / (24 * HourLengthSeconds));
        string dayStr = DisplayDay(days);

        float hours = d % (24 * HourLengthSeconds);
        hours /= HourLengthSeconds;
        string hourStr = DisplayTime(hours);

        return $"{dayStr} {hourStr}";
    }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        if(forceTimePause)
            Debug.LogWarning("Time is paused in the DayNightCycleController");
    }

    public void Clear()
    {
        timeOfDay = gameStartTime;
        day = 1;
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        timeOfDay = gameStartTime;
        day = 1;
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.timeData = new TimeOfDaySaveData { timeOfDay = TimeOfDay, day = Day };
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.timeData != null)
        {
            timeOfDay = data.timeData.timeOfDay;
            day = data.timeData.day;
        }

        return null;
    }

    void Update()
    {
        if(Application.isPlaying && !forceTimePause)
        {
            timeOfDay += Time.deltaTime / HourLengthSeconds;
            
            if(TimeOfDay > 24f)
            {
                day++;
                timeOfDay %= 24f;
            }
        }

        float tod = seasonCurve.Evaluate(TimeOfDay / 24f);
        float angle = 360f * tod - 90f + sunAngleOffset;

        var ambientLight = ambientLightColor.Evaluate(tod);
        RenderSettings.ambientLight = ambientLight;
        RenderSettings.fogColor = fog.Evaluate(tod);

        transform.rotation = Quaternion.Euler(0f, -90f, 30f);

        var directionalLightColor = directionalLight.Evaluate(tod);
        sunLight.color = directionalLightColor;
        sunLight.transform.localRotation = Quaternion.Euler(angle, 0f, 0f);

        lightLevel = (directionalLightColor.grayscale + ambientLight.grayscale) / 2;

        foreach (var mat in sunSensitiveMaterials)
        {
            mat.SetVector("_sunDirection", sunLight.transform.forward);
            mat.SetVector("_sunColor", sunColor.Evaluate(tod));
            mat.SetVector("_ambientLightColor", ambientLight);
            mat.SetVector("_directionalLightColor", ambientLight);
        }
    }
}
