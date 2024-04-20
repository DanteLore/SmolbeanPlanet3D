using System;

[Serializable]
public class DataCollectionSeriesSaveData
{
    public string seriesName;
    public DataCollectionReading[] readings;
    public float minValue;
    public float maxValue;
}
