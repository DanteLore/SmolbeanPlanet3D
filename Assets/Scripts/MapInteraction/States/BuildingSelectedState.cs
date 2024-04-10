using System.Linq;
using UnityEngine;

public class BuildingSelectedState : IState
{
    private readonly MapInteractionManager parent;
    private readonly GameObject selectionCursorPrefab;
    private GameObject cursor;

    public BuildingSelectedState(MapInteractionManager parent, GameObject selectionCursorPrefab)
    {
        this.parent = parent;
        this.selectionCursorPrefab = selectionCursorPrefab;
    }

    public void OnEnter()
    {
        cursor = Object.Instantiate(selectionCursorPrefab, parent.SelectedTransform);
        var pos = cursor.transform.position;
        float y = GetBounds(parent.SelectedTransform.gameObject).max.y + 1f;
        pos = new Vector3(pos.x, y, pos.z);
        cursor.transform.position = pos;

        MenuController.Instance.ShowMenu("BuildingDetailsMenu");
    }

    public void OnExit()
    {
        Object.Destroy(cursor);
        cursor = null;
        MenuController.Instance.Close("BuildingDetailsMenu");
    }

    public void Tick()
    {
    }

    private static Bounds GetBounds(GameObject building)
    {
        var allBounds = building.transform.GetComponentsInChildren<Renderer>().Select(r => r.bounds).ToArray();

        var bounds = allBounds[0];

        for (int i = 1; i < allBounds.Length; i++)
            bounds.Encapsulate(allBounds[i]);

        return bounds;
    }
}
