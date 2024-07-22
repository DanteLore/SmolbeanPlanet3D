using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LineChart : VisualElement
{
    public LineChart()
    {
        this.StretchToParentSize();
        generateVisualContent += OnGenerateVisualContent;
    }

    private DataCollectionSeries[] series;
    private float drawWidth;
    private float drawHeight;
    private float minValue;
    private float maxValue;
    private float startTime;
    private float endTime;

    private const int MAX_READINGS_PER_LINE = 100;

    public DataCollectionSeries[] Series
    {
        get { return series; }
        set 
        {
            if(series != null)
                foreach(var s in series)
                    s.OnValuesChanged -= SeriesChanged;

            series = value; 
            MarkDirtyRepaint();

            if(series != null)
                foreach(var s in series)
                    s.OnValuesChanged += SeriesChanged;
        }
    }

    private void SeriesChanged()
    {
        MarkDirtyRepaint();
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;

        GetChartDrawArea();
        GetMinMaxValues();
        DrawGridlines(painter);
        DrawAxes(painter);
        DrawSeries(painter);
    }

    private void DrawSeries(Painter2D painter)
    {
        foreach (var s in Series)
        {
            if (!s.IsVisible)
                continue;

            painter.strokeColor = s.lineColor;
            painter.lineJoin = LineJoin.Round;
            painter.lineCap = LineCap.Round;
            painter.lineWidth = 2.0f;

            int batchOffset = 0;
            while(batchOffset < s.Readings.Count)
            {
                DrawSeriesSubset(painter, s, batchOffset, MAX_READINGS_PER_LINE);
                batchOffset += MAX_READINGS_PER_LINE - 1;
            }
        }
    }

    private void DrawSeriesSubset(Painter2D painter, DataCollectionSeries series, int index, int count)
    {
        painter.BeginPath();
        var start = new Vector2(NormaliseX(series.Readings[index].startTime), NormaliseY(series.Readings[index].value));
        var end = new Vector2(NormaliseX(series.Readings[index].endTime), NormaliseY(series.Readings[index].value));
        painter.MoveTo(start);
        painter.LineTo(end);

        int i = 1;
        while(i < count && index + i < series.Readings.Count)
        {
            start = new Vector2(NormaliseX(series.Readings[index + i].startTime), NormaliseY(series.Readings[index + i].value));
            end = new Vector2(NormaliseX(series.Readings[index + i].endTime), NormaliseY(series.Readings[index + i].value));
            painter.LineTo(start);
            painter.LineTo(end);
            i++;
        }
        painter.Stroke();
    }

    private void DrawAxes(Painter2D painter)
    {
        // Axes
        painter.BeginPath();
        painter.lineWidth = 2.0f;
        painter.strokeColor = new Color(1f, 1f, 1f, 0.2f);
        painter.MoveTo(new Vector2(0f, drawHeight));
        painter.LineTo(new Vector2(drawWidth, drawHeight));
        painter.MoveTo(new Vector2(0f, 0f));
        painter.LineTo(new Vector2(0f, drawHeight));
        painter.Stroke();
    }

    private void DrawGridlines(Painter2D painter)
    {
        painter.BeginPath();
        painter.lineWidth = 1.0f;
        painter.strokeColor = new Color(1f, 1f, 1f, 0.1f);

        // Vertical 
        float duration = endTime - startTime;
        float dayLength = DayNightCycleController.Instance.HourLengthSeconds * 24;
        float horizontalStep = duration < dayLength ? DayNightCycleController.Instance.HourLengthSeconds : dayLength;
        float x = Mathf.Ceil(startTime / horizontalStep) * horizontalStep;
        while (x < endTime)
        {
            var start = new Vector2(NormaliseX(x), NormaliseY(minValue));
            var end = new Vector2(NormaliseX(x), NormaliseY(maxValue));
            painter.MoveTo(start);
            painter.LineTo(end);
            x += horizontalStep;
        }

        // Horizontal
        float range = maxValue - minValue;
        float verticalStep = Mathf.Ceil(range / 100) * 10;
        float y = Mathf.Ceil(minValue / verticalStep) * verticalStep;
        while (y < maxValue)
        {
            var start = new Vector2(NormaliseX(startTime), NormaliseY(y));
            var end = new Vector2(NormaliseX(endTime), NormaliseY(y));
            painter.MoveTo(start);
            painter.LineTo(end);
            y += verticalStep;
        }

        painter.Stroke();
    }

    private void GetMinMaxValues()
    {
        // This gets called quite a bit, so don't use Linq ;)
        minValue = float.MaxValue;
        maxValue = float.MinValue;
        startTime = float.MaxValue;
        endTime = float.MinValue;
        
        for (int i = 0; i < Series.Length; i++)
        {
            if(series[i].IsVisible)
            {
                if (series[i].MaxValue > maxValue)
                    maxValue = series[i].MaxValue;
                if (series[i].MinValue < minValue)
                    minValue = series[i].MinValue;
                if(series[i].StartTime < startTime)
                    startTime = series[i].StartTime;
                if(series[i].EndTime > endTime)
                    endTime = series[i].EndTime;
            }
        }

        // No compressed Y axes here
        minValue = Mathf.Min(0f, minValue);
    }

    private void GetChartDrawArea()
    {
        var rect = contentRect; // Very expensive call to a property.  Only do it once!
        drawWidth = rect.width;
        drawHeight = rect.height;
    }

    private float NormaliseX(float time)
    {
        float dx = Mathf.InverseLerp(startTime, endTime, time);
        return Mathf.Lerp(0.0f, drawWidth, dx);
    }

    private float NormaliseY(float value)
    {
        float dy = Mathf.InverseLerp(minValue, maxValue, value);
        return (float)Mathf.Lerp(0, drawHeight, 1 - dy);
    }
}
