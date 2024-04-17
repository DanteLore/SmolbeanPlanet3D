using UnityEngine;
using UnityEngine.UIElements;

public class LineChart : VisualElement
{
    public LineChart()
    {
        generateVisualContent += OnGenerateVisualContent;
    }
     
    void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;

        painter.strokeColor = Color.white;
        painter.lineJoin = LineJoin.Round;
        painter.lineCap = LineCap.Round;
        painter.lineWidth = 10.0f;
        painter.BeginPath();
        painter.MoveTo(new Vector2(100, 100));
        painter.LineTo(new Vector2(120, 120));
        painter.LineTo(new Vector2(140, 100));
        painter.LineTo(new Vector2(160, 120));
        painter.LineTo(new Vector2(180, 100));
        painter.LineTo(new Vector2(200, 120));
        painter.LineTo(new Vector2(220, 100));
        painter.Stroke();
    }
}
