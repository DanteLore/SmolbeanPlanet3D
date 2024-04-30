using UnityEngine;

public class ChooseBuildingLocationState : BaseMapInteractionState
{
    private const float allowedHeightDifferential = 0.5f;
    private const float cursorSize = 4f;
    private readonly Transform parent;
    private readonly GameObject buildingPlacementCursorPrefab;
    private readonly GridManager gridManager;
    private readonly string groundLayer;
    private readonly string[] collisionLayers;

    private GameObject cursor;
    private Vector2Int currentSquare;

    public bool okToBuild;
    private Vector3 buildingSize;

    public ChooseBuildingLocationState(
        MapInteractionData data,
        Transform parent,
        GameObject buildingPlacementCursorPrefab,
        GridManager gridManager,
        string groundLayer,
        string[] collisionLayers)
        : base(data)
    {
        this.parent = parent;
        this.buildingPlacementCursorPrefab = buildingPlacementCursorPrefab;
        this.gridManager = gridManager;
        this.groundLayer = groundLayer;
        this.collisionLayers = collisionLayers;
    }

    public override void OnEnter()
    {
        cursor = Object.Instantiate(buildingPlacementCursorPrefab, data.HitPoint, Quaternion.identity, parent);

        GetLookRotation();

        var prefab = data.SelectedBuildingSpec.prefab;
        var bounds = prefab.GetRendererBounds(activeOnly: false);
        buildingSize = RotateBuildingBounds(bounds.size, data.BuildingRotationY);
    }

    private Vector3 RotateBuildingBounds(Vector3 size, float angle)
    {
        // Angle will always be a right angle
        if(angle % 180f > 45f)
            return new Vector3(size.z, size.y, size.x);
        else
            return size;
    }

    private void GetLookRotation()
    {
        float rotationY = Camera.main.transform.rotation.eulerAngles.y;
        rotationY += 180f; // Opposite
        rotationY %= 360f;
        rotationY = Mathf.Round(rotationY / 90f) * 90f; // Round to nearest 90deg
        data.BuildingRotationY = rotationY;
    }

    public override void OnExit()
    {
        Object.Destroy(cursor);
    }

    public override void Tick()
    {
        base.Tick();

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out var hitInfo, float.MaxValue, LayerMask.GetMask(groundLayer)))
        {
            cursor.SetActive(false);
            return;
        }

        Vector2Int newSquare = gridManager.GetGameSquareFromWorldCoords(hitInfo.point);

        if (newSquare != currentSquare)
            NewSquareSelected(newSquare);

        currentSquare = newSquare;
    }

    private void NewSquareSelected(Vector2Int newSquare)
    {
        Rect squareBounds = gridManager.GetSquareBounds(newSquare.x, newSquare.y);

        float worldX = squareBounds.center.x;
        float worldZ = squareBounds.center.y;
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

        if (float.IsNaN(worldY))
            return;

        var center = new Vector3(worldX, worldY, worldZ);

        Bounds bounds = new(center, buildingSize);

        okToBuild = CheckFlat(bounds) && CheckEmpty(bounds);

        cursor.transform.position = center;
        cursor.transform.localScale = new Vector3(buildingSize.x / cursorSize, 1f, buildingSize.z / cursorSize);
        cursor.GetComponent<Renderer>().material.SetColor("_baseColor", okToBuild ? Color.blue : Color.red);
        cursor.SetActive(true);
        data.SelectedPoint = center;
    }

    private bool CheckEmpty(Bounds bounds)
    {
        var objects = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, LayerMask.GetMask(collisionLayers));
        return objects.Length == 0;
    }

    private bool CheckFlat(Bounds bounds)
    {
        float rayStartHeight = 10000f;
        int groundMask = LayerMask.GetMask(groundLayer);

        // Slightly off center, in case there's a small gap between meshes!
        var ray0 = new Ray(new Vector3(bounds.center.x + 0.01f, rayStartHeight, bounds.center.z + 0.01f), Vector3.down);
        if (!Physics.Raycast(ray0, out var hit0, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 0: " + hit0.point);

        float height = hit0.point.y;

        var ray1 = new Ray(new Vector3(bounds.min.x, rayStartHeight, bounds.min.z), Vector3.down);
        if (!Physics.Raycast(ray1, out var hit1, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 1: " + hit1.point);

        if (Mathf.Abs(hit1.point.y - height) > allowedHeightDifferential)
            return false;

        var ray2 = new Ray(new Vector3(bounds.max.x, rayStartHeight, bounds.min.z), Vector3.down);
        if (!Physics.Raycast(ray2, out var hit2, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 2: " + hit2.point);

        if (Mathf.Abs(hit2.point.y - height) > allowedHeightDifferential)
            return false;

        var ray3 = new Ray(new Vector3(bounds.min.x, rayStartHeight, bounds.max.z), Vector3.down);
        if (!Physics.Raycast(ray3, out var hit3, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 3: " + hit3.point);

        if (Mathf.Abs(hit3.point.y - height) > allowedHeightDifferential)
            return false;

        var ray4 = new Ray(new Vector3(bounds.max.x, rayStartHeight, bounds.max.z), Vector3.down);
        if (!Physics.Raycast(ray4, out var hit4, float.PositiveInfinity, groundMask))
            return false;
        //Debug.Log("Hit 4: " + hit4.point);

        if (Mathf.Abs(hit4.point.y - height) > allowedHeightDifferential)
            return false;

        return true;
    }
}
