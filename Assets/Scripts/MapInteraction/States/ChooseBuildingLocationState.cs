using UnityEngine;

public class ChooseBuildingLocationState : IState
{
    private const float allowedHeightDifferential = 0.2f;
    private readonly MapInteractionData data;
    private readonly Transform parent;
    private readonly GameObject buildingPlacementCursorPrefab;
    private readonly GridManager gridManager;
    private readonly string groundLayer;
    private readonly string[] collisionLayers;

    private GameObject cursor;
    private Vector2Int currentSquare;

    public bool okToBuild;

    public ChooseBuildingLocationState(
        MapInteractionData data,
        Transform parent,
        GameObject buildingPlacementCursorPrefab,
        GridManager gridManager,
        string groundLayer,
        string[] collisionLayers)
    {
        this.data = data;
        this.parent = parent;
        this.buildingPlacementCursorPrefab = buildingPlacementCursorPrefab;
        this.gridManager = gridManager;
        this.groundLayer = groundLayer;
        this.collisionLayers = collisionLayers;
    }

    public void OnEnter()
    {
        cursor = Object.Instantiate(buildingPlacementCursorPrefab, data.HitPoint, Quaternion.identity, parent);
    }

    public void OnExit()
    {
        Object.Destroy(cursor);
    }

    public void Tick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask(groundLayer)))
        {
            cursor.SetActive(false);
            return;
        }

        Vector2Int newSquare = gridManager.GetGameSquareFromWorldCoords(hitInfo.point);

        if (newSquare != currentSquare)
        {
            currentSquare = newSquare;

            Rect squareBounds = gridManager.GetSquareBounds(currentSquare.x, currentSquare.y);
            var prefab = data.SelectedBuildingSpec.prefab;
            Bounds buildingBounds = prefab.GetComponentInChildren<MeshRenderer>().bounds;

            for (int i = 0; i < prefab.transform.childCount; i++)
                buildingBounds.Encapsulate(prefab.transform.GetChild(i).position);
            buildingBounds.Expand(0.75f);

            float worldX = squareBounds.center.x;
            float worldZ = squareBounds.center.y;
            float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

            Bounds bounds = new(squareBounds.center, buildingBounds.size);

            if (!float.IsNaN(worldY))
            {
                var center = new Vector3(worldX, worldY, worldZ);
                okToBuild = CheckFlat(bounds) && CheckEmpty(center);

                Color color = okToBuild ? Color.blue : Color.red;
                cursor.transform.position = center;
                cursor.GetComponent<Renderer>().material.SetColor("_baseColor", color);
                cursor.SetActive(true);
                data.SelectedPoint = center;
            }
        }
    }

    private bool CheckEmpty(Vector3 center)
    {
        var halfExtents = new Vector3(gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f);
        var objects = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask(collisionLayers));
        return objects.Length == 0;
    }

    private bool CheckFlat(Bounds bounds)
    {
        float rayStartHeight = 10000f;
        int groundMask = LayerMask.GetMask(groundLayer);

        var ray0 = new Ray(new Vector3(bounds.center.x, rayStartHeight, bounds.center.y), Vector3.down);
        if (!Physics.Raycast(ray0, out var hit0, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 0: " + hit0.point);

        float height = hit0.point.y;

        var ray1 = new Ray(new Vector3(bounds.min.x, rayStartHeight, bounds.min.y), Vector3.down);
        if (!Physics.Raycast(ray1, out var hit1, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 1: " + hit1.point);

        if (Mathf.Abs(hit1.point.y - height) > allowedHeightDifferential)
            return false;

        var ray2 = new Ray(new Vector3(bounds.max.x, rayStartHeight, bounds.min.y), Vector3.down);
        if (!Physics.Raycast(ray2, out var hit2, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 2: " + hit2.point);

        if (Mathf.Abs(hit2.point.y - height) > allowedHeightDifferential)
            return false;

        var ray3 = new Ray(new Vector3(bounds.min.x, rayStartHeight, bounds.max.y), Vector3.down);
        if (!Physics.Raycast(ray3, out var hit3, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 3: " + hit3.point);

        if (Mathf.Abs(hit3.point.y - height) > allowedHeightDifferential)
            return false;

        var ray4 = new Ray(new Vector3(bounds.max.x, rayStartHeight, bounds.max.y), Vector3.down);
        if (!Physics.Raycast(ray4, out var hit4, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 4: " + hit4.point);

        if (Mathf.Abs(hit4.point.y - height) > allowedHeightDifferential)
            return false;

        return true;
    }
}
