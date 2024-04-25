using UnityEngine;

public class AnimalSelectedState : BaseMapInteractionState
{
    private readonly GameObject selectionCursorPrefab;
    private GameObject cursor;
    private Transform target;

    public AnimalSelectedState(MapInteractionData data, GameObject selectionCursorPrefab) : base(data)
    {
        this.selectionCursorPrefab = selectionCursorPrefab;
    }

    public override void OnEnter()
    {
        target = data.SelectedTransform;

        if(target.TryGetComponent<FollowCameraTarget>(out var followCameraTarget))
            FollowCameraController.Instance.SetTarget(followCameraTarget);
        else
            cursor = Object.Instantiate(selectionCursorPrefab, target);
    
        MenuController.Instance.ShowMenu("AnimalDetailsMenu");
    }

    public override void OnExit()
    {
        FollowCameraController.Instance.SetTarget(null);

        if(cursor != null)
        {
            Object.Destroy(cursor);
            cursor = null;
        }
        
        MenuController.Instance.Close("AnimalDetailsMenu");
    }
}
