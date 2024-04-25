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

        if(target != null && target.TryGetComponent<SmolbeanColonist>(out var colonist))
            colonist.IdentitySwapped += ColonistIdentitySwapped;

        if(target.TryGetComponent<FollowCameraTarget>(out var followCameraTarget))
            FollowCameraController.Instance.SetTarget(followCameraTarget);
        else
            cursor = Object.Instantiate(selectionCursorPrefab, target);
    
        MenuController.Instance.ShowMenu("AnimalDetailsMenu");
    }

    public override void OnExit()
    {
        if(target != null && target.TryGetComponent<SmolbeanColonist>(out var colonist))
            colonist.IdentitySwapped -= ColonistIdentitySwapped;

        FollowCameraController.Instance.SetTarget(null);

        if(cursor != null)
        {
            Object.Destroy(cursor);
            cursor = null;
        }
        
        MenuController.Instance.Close("AnimalDetailsMenu");
    }

    private void ColonistIdentitySwapped(SmolbeanColonist orignal, SmolbeanColonist replacement)
    {
        data.ForceSelect(replacement.transform);
    }
}
