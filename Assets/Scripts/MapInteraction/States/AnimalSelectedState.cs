using UnityEngine;

public class AnimalSelectedState : BaseMapInteractionState
{
    private readonly GameObject selectionCursorPrefab;
    private GameObject cursor;

    public AnimalSelectedState(MapInteractionData data, GameObject selectionCursorPrefab) : base(data)
    {
        this.selectionCursorPrefab = selectionCursorPrefab;
    }

    public override void OnEnter()
    {
        cursor = Object.Instantiate(selectionCursorPrefab, data.SelectedTransform);
        MenuController.Instance.ShowMenu("AnimalDetailsMenu");
    }

    public override void OnExit()
    {
        Object.Destroy(cursor);
        cursor = null;
        MenuController.Instance.Close("AnimalDetailsMenu");
    }
}
