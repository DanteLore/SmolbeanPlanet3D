using System.Collections.Generic;

public class DataCollectionSeries
{
    private readonly List<float> values = new();
    private float minValue = float.MaxValue;
    private float maxValue = float.MinValue;

    public IEnumerable<float> Values { get { return values; } }
    public float MinValue { get { return minValue; } }
    public float MaxValue { get { return maxValue; } }

    public void AddValue(float value)
    {
        if(value > maxValue)
            maxValue = value;
        if(value < minValue)
            minValue = value;

        values.Add(value);
    }
}
