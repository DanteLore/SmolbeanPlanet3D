using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LineChart : VisualElement
{
    public new class UxmlFactory : UxmlFactory<LineChart, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
        }
    }

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

        foreach (var s in Series)
        {
            painter.strokeColor = s.lineColor;
            painter.lineJoin = LineJoin.Round;
            painter.lineCap = LineCap.Round;
            painter.lineWidth = 2.0f;

            painter.BeginPath();
            var start = new Vector2((float)NormaliseX(s.Readings[0].startTime), (float)NormaliseY(s.Readings[0].value));
            var end = new Vector2((float)NormaliseX(s.Readings[0].endTime), (float)NormaliseY(s.Readings[0].value));
            painter.MoveTo(start);
            painter.LineTo(end);
            
            for (int i = 1; i < s.Readings.Count; i++)
            {
                start = new Vector2((float)NormaliseX(s.Readings[i].startTime), (float)NormaliseY(s.Readings[i].value));
                end = new Vector2((float)NormaliseX(s.Readings[i].endTime), (float)NormaliseY(s.Readings[i].value));
                painter.LineTo(start);
                painter.LineTo(end);
            }
            painter.Stroke();
        }
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
