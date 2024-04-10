using UnityEngine;

public class AnimalSelectedState : IState
{
    private readonly MapInteractionManager parent;
    private readonly GameObject selectionCursorPrefab;
    private GameObject cursor;

    public AnimalSelectedState(MapInteractionManager parent, GameObject selectionCursorPrefab)
    {
        this.parent = parent;
        this.selectionCursorPrefab = selectionCursorPrefab;
    }

    public void OnEnter()
    {
        cursor = Object.Instantiate(selectionCursorPrefab, parent.SelectedTransform);
        MenuController.Instance.ShowMenu("AnimalDetailsMenu");
    }

    public void OnExit()
    {
        Object.Destroy(cursor);
        cursor = null;
        MenuController.Instance.Close("AnimalDetailsMenu");
    }

    public void Tick()
    {
    }
}
