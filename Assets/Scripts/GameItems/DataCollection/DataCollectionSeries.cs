using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataCollectionSeries : MonoBehaviour
{
    public string seriesName;
    public string displayName;
    public string description;
    public Texture2D thumbnail;
    public Color lineColor;
    public int maxSamples = 1000;
    
    private readonly List<float> values = new();
    private float minValue = float.MaxValue;
    private float maxValue = float.MinValue;

    public IReadOnlyList<float> Values { get { return values; } }
    public float MinValue { get { return minValue; } }
    public float MaxValue { get { return maxValue; } }

    public event Action OnValuesChanged;

    public void AddValue()
    {
        float value = GetDataValue();

        if(value > maxValue)
            maxValue = value;
        if(value < minValue)
            minValue = value;

        values.Add(value);

        while(values.Count > maxSamples)
            values.RemoveAt(0);

        OnValuesChanged?.Invoke();
    }

    protected abstract float GetDataValue();
}
