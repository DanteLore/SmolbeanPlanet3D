using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DataCollectionSeries : MonoBehaviour
{
    public string seriesGroup = "Other";
    public string seriesName;
    public string description;
    public Color lineColor;
    public int maxSamples = 100000;
    
    private readonly List<DataCollectionReading> readings = new();
    
    public IReadOnlyList<DataCollectionReading> Readings { get { return readings; } }
    public float MinValue { get; private set; } = float.MaxValue;
    public float MaxValue { get; private set; } = float.MinValue;
    public float StartTime { get { return readings[0].startTime; } }
    public float EndTime { get { return readings[^1].endTime; } }
    public bool IsVisible { get; private set; } = true;

    public event Action OnValuesChanged;

    public void AddValue(float time)
    {
        float value = GetDataValue();

        if(value > MaxValue)
            MaxValue = value;
        if(value < MinValue)
            MinValue = value;

        if(readings.Count == 0 || readings[^1].value != value)
            readings.Add(new DataCollectionReading { startTime = time, endTime = time, value = value });
        else
            readings[^1].endTime = time;

        while(readings.Count > maxSamples)
            readings.RemoveAt(0);

        OnValuesChanged?.Invoke();
    }

    protected abstract float GetDataValue();

    public void Clear()
    {
        readings.Clear();

        MinValue = float.MaxValue;
        MaxValue = float.MinValue;
    }

    public DataCollectionSeriesSaveData GetSaveData()
    {   
        return new DataCollectionSeriesSaveData 
        { 
            seriesName = seriesName, 
            readings = Readings.ToArray(), 
            minValue = MinValue, 
            maxValue = MaxValue 
        };
    }

    public void LoadFrom(DataCollectionSeriesSaveData seriesData)
    {
        Clear();

        readings.AddRange(seriesData.readings);
        MaxValue = seriesData.maxValue;
        MinValue = seriesData.minValue;
    }

    public void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        OnValuesChanged?.Invoke();
    }
}
