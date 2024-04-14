using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class BaseMapInteractionState : IState
{
    protected readonly MapInteractionData data;

    protected BaseMapInteractionState(MapInteractionData data)
    {
        this.data = data;
    }

    public abstract void OnEnter();

    public abstract void OnExit();

    public virtual void Tick()
    {
        data.ClearSingleFrameFlags();
        CaptureMouseState();
        CheckSelectItem();
    }

    protected void CheckSelectItem()
    {
        if (!data.OverMenu)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, float.MaxValue, data.AllLayers))
            {
                data.HitPoint = hit.point;

                if (data.LeftButtonClicked)
                {
                    if (!ReferenceEquals(data.SelectedTransform, hit.transform))
                        data.SetNewObjectClicked();
                    data.SelectedTransform = hit.transform;
                }
            }
        }
    }

    protected void CaptureMouseState()
    {
        data.OverMenu = EventSystem.current.IsPointerOverGameObject();
        data.LeftButtonClicked = Mouse.current.leftButton.wasPressedThisFrame;
        data.RightButtonClicked = Mouse.current.rightButton.wasPressedThisFrame;
    }
}
