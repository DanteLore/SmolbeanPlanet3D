using System.Collections.Generic;

public class DataCollectionSeries
{
    private readonly List<float> values = new();
    private float minValue = float.MaxValue;
    private float maxValue = float.MinValue;
    private int maxSamples = 1000;

    public IReadOnlyList<float> Values { get { return values; } }
    public float MinValue { get { return minValue; } }
    public float MaxValue { get { return maxValue; } }

    public void AddValue(float value)
    {
        if(value > maxValue)
            maxValue = value;
        if(value < minValue)
            minValue = value;

        values.Add(value);

        while(values.Count > maxSamples)
            values.RemoveAt(0);
    }
}
