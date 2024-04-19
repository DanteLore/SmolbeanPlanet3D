using System;
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

    private DataCollectionSeries series;
    public DataCollectionSeries Series
    {
        get { return series; }
        set 
        {
            if(series != null)
                series.OnValuesChanged -= SeriesChanged;

            series = value; 
            MarkDirtyRepaint();

            if(series != null)
                series.OnValuesChanged += SeriesChanged;
        }
    }

    private void SeriesChanged()
    {
        MarkDirtyRepaint();
    }

    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;

        painter.strokeColor = Series.lineColor;
        painter.lineJoin = LineJoin.Round;
        painter.lineCap = LineCap.Round;
        painter.lineWidth = 2.0f;
  
        painter.BeginPath();
        painter.MoveTo(VectorAt(0));
        for(int i = 1; i < Series.Values.Count; i++)
            painter.LineTo(VectorAt(i));
        painter.Stroke();
    }

    private Vector2 VectorAt(int i)
    {
        float x = NormaliseX(i);
        float y = NormaliseY(Series.Values[i]);
        var v = new Vector2(x, y);
        return v;
    }

    private float NormaliseX(float v)
    {
        float dx = v / Series.Values.Count;
        return (float)Mathf.Lerp(0, contentRect.width, dx);
    }

    private float NormaliseY(float v)
    {
        float dy = Mathf.InverseLerp(Series.MinValue, Series.MaxValue, v);
        return (float)Mathf.Lerp(0, contentRect.height, 1 - dy);
    }
}
