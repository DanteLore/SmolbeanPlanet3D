using UnityEngine;

public class AnimalSelectedState : IState
{
    private readonly MapInteractionData data;
    private readonly GameObject selectionCursorPrefab;
    private GameObject cursor;

    public AnimalSelectedState(MapInteractionData data, GameObject selectionCursorPrefab)
    {
        this.data = data;
        this.selectionCursorPrefab = selectionCursorPrefab;
    }

    public void OnEnter()
    {
        cursor = Object.Instantiate(selectionCursorPrefab, data.SelectedTransform);
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
