using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class DayNightCycleController : MonoBehaviour, IObjectGenerator
{
    public static DayNightCycleController Instance { get; private set; }
    public bool RunModeOnly { get { return true; } }
    public Gradient ambientLightColor;
    [GradientUsage(hdr:true)] public Gradient sunColor;
    public Gradient directionalLight;
    public Gradient fog;
    public AnimationCurve seasonCurve;
    public Light sunLight;
    public Material[] sunSensitiveMaterials;
    public float hourLengthSeconds = 12f;
    public float lightLevel;

    [Range(0f, 24f)]
    public float gameStartTime = 7f;

    [Range(0f, 24f)]
    public float timeOfDay;
    public int day = 1;
    public int Priority { get { return 4; } }

    public bool TimeIsBetween(float start, float end)
    {
        if (start < end)
            return start <= timeOfDay && timeOfDay <= end;
        else
            return timeOfDay <= end || timeOfDay >= start;
    }

    public string DisplayTime(float t = -1f)
    {
        float tod = t == -1 ? timeOfDay : t;

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
        int x = d == -1 ? day : d;
        return $"🜳 {x}";
    }

    public string DurationToString(float d)
    {
        int days = Mathf.FloorToInt(d / (24 * hourLengthSeconds));
        string dayStr = DisplayDay(days);

        float hours = d % (24 * hourLengthSeconds);
        hours /= hourLengthSeconds;
        string hourStr = DisplayTime(hours);

        return $"{dayStr} {hourStr}";
    }

    public string DisplayTimeAndDay
    {
        get
        {
            return $"{DisplayDay()}   {DisplayTime()}";
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

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        timeOfDay = gameStartTime;
        day = 1;
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.timeData = new TimeOfDaySaveData { timeOfDay = timeOfDay, day = day };
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