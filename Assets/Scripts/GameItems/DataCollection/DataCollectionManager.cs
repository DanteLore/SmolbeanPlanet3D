using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataCollectionManager : MonoBehaviour, IObjectGenerator
{
    public static DataCollectionManager Instance;
    
    public int Priority { get { return 500; } }
    public bool RunModeOnly { get { return true; } }

    public List<DataCollectionSeries> Series {get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Clear()
    {
        StopCoroutine(nameof(FetchDataLoop));

        InitialiseSeries();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        InitialiseSeries();

        StartCoroutine(nameof(FetchDataLoop));

        yield return null;
    }

    private void InitialiseSeries()
    {
        Series = FindObjectsByType<DataCollectionSeries>(sortMode: FindObjectsSortMode.None).OrderBy(s => s.seriesName).ToList();
        foreach (var series in Series)
            series.Clear();
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        InitialiseSeries();

        if(data.dataCollectionSeries != null)
        {
            foreach(var seriesData in data.dataCollectionSeries)
            {
                var series = Series.FirstOrDefault(s => s.seriesName == seriesData.seriesName);
                if(series != null)
                {
                    series.LoadFrom(seriesData);
                }
                yield return null;
            }
        }

        StartCoroutine(nameof(FetchDataLoop));

        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.dataCollectionSeries = Series.Select(s => s.GetSaveData()).ToList();
    }

    private IEnumerator FetchDataLoop()
    {
        yield return new WaitForSeconds(1f);
        Debug.Assert(Series.Count < 30, "Getting enough data collection series to need a new algo here!");

        while(true)
        {
            float startTime = Time.time;

            // Yeah yeah yeah... this is a weird way to store the time... I know... I know
            float gameTime = DayNightCycleController.Instance.Day * 24 + DayNightCycleController.Instance.TimeOfDay;

            foreach(var series in Series)
            {
                series.AddValue(gameTime);
                yield return null;
            }
            float timeElapsed =  Time.time - startTime;

            yield return new WaitForSeconds(1f - timeElapsed);
        }
    }
}
