using UnityEngine;
using UnityEngine.UIElements;

// Thanks pbhogan!  https://gist.github.com/pbhogan/2094a033c094ddd1b0b8f37a5dffd005
// Update for unity 6 => https://discussions.unity.com/t/will-2023-2-uxmlfactory-and-uxmltraits-obsolete-yes/924312

[UxmlElement]
public partial class AspectRatioPanel : VisualElement
{
    [UxmlAttribute]
    public int AspectRatioX { get; private set; } = 16;
    [UxmlAttribute]
    public int AspectRatioY { get; private set; } = 9;
    [UxmlAttribute]
    public int BalanceX { get; private set; } = 50;
    [UxmlAttribute]
    public int BalanceY { get; private set; } = 50;

    public AspectRatioPanel()
    {
        style.position = Position.Absolute;
        style.left = 0;
        style.top = 0;
        style.right = StyleKeyword.Undefined;
        style.bottom = StyleKeyword.Undefined;
        RegisterCallback<AttachToPanelEvent>( OnAttachToPanelEvent );
    }

    void OnAttachToPanelEvent( AttachToPanelEvent e )
    {
        parent?.RegisterCallback<GeometryChangedEvent>( OnGeometryChangedEvent );
        FitToParent();
    }

    void OnGeometryChangedEvent( GeometryChangedEvent e )
    {
        FitToParent();
    }

    void FitToParent()
    {
        if (parent == null) return;
        var parentW = parent.resolvedStyle.width;
        var parentH = parent.resolvedStyle.height;
        if (float.IsNaN( parentW ) || float.IsNaN( parentH )) return;

        style.position = Position.Absolute;
        style.left = 0;
        style.top = 0;
        style.right = StyleKeyword.Undefined;
        style.bottom = StyleKeyword.Undefined;

        if (AspectRatioX <= 0.0f || AspectRatioY <= 0.0f)
        {
            style.width = parentW;
            style.height = parentH;
            return;
        }

        var ratio = Mathf.Min( parentW / AspectRatioX, parentH / AspectRatioY );
        var targetW = Mathf.Floor( AspectRatioX * ratio );
        var targetH = Mathf.Floor( AspectRatioY * ratio );
        style.width = targetW;
        style.height = targetH;

        var marginX = parentW - targetW;
        var marginY = parentH - targetH;
        style.left = Mathf.Floor( marginX * BalanceX / 100.0f );
        style.top = Mathf.Floor( marginY * BalanceY / 100.0f );
    }
}