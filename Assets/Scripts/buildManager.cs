using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildManager : MonoBehaviour
{
    public GameObject mapCursorPrefab;
    public string groundLayer = "Ground";
    public string[] collisionLayers = { "Nature", "Buildings", "Creatures" };

    private GridManager gridManager;
    private GameMapGenerator gameMapGenerator;
    private Vector2Int currentSquare;

    private GameObject mapCursor;

    void Start()
    {
        gridManager = GetComponent<GridManager>();
        gameMapGenerator = GetComponent<GameMapGenerator>();
        currentSquare = new Vector2Int(int.MaxValue, int.MaxValue);

        mapCursor = Instantiate(mapCursorPrefab, Vector3.zero, Quaternion.identity, transform);
        mapCursor.SetActive(false);
    }

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hitInfo, LayerMask.GetMask(groundLayer)))
        {
            Vector2Int newSquare = gridManager.GetGameSquareFromWorldCoords(hitInfo.point);

            if(newSquare != currentSquare)
            {
                currentSquare = newSquare;

                int level = gameMapGenerator.GameMap[currentSquare.y * gameMapGenerator.mapWidth + currentSquare.x];

                if(level > 0)
                {
                    var bounds = gridManager.GetSquareBounds(currentSquare.x, currentSquare.y);

                    float worldX = bounds.center.x;
                    float worldZ = bounds.center.y;
                    float worldY = gridManager.GetGridHeightAt(worldX, worldZ);
                    var center = new Vector3(worldX, worldY, worldZ);

                    Color color = CheckFlat(bounds) && CheckEmpty(center) ? Color.blue : Color.red;

                    mapCursor.transform.position = center;
                    mapCursor.GetComponent<Renderer>().material.SetColor("_baseColor", color);
                    mapCursor.SetActive(true);
                }
                else
                {
                    mapCursor.SetActive(false);
                }
            }
        }
    }

    private bool CheckEmpty(Vector3 center)
    {
        Vector3 boxCentre = new Vector3(center.x, center.y + gridManager.tileSize / 2.0f, center.z);
        Vector3 halfExtents = new Vector3(gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f, gridManager.tileSize / 2.0f);
        var objects = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask(collisionLayers));
        return objects.Length == 0;
    }

    private bool CheckFlat(Rect bounds)
    {
        float marginSize = 0.05f;
        float allowedHeightDifferential = 0.2f;
        float rayStartHeight = 1000f;

        float rayLength = 2.0f * rayStartHeight;
        float margin = bounds.width * marginSize;

        var ray1 = new Ray(new Vector3(bounds.xMin + margin, rayStartHeight, bounds.yMin + margin), Vector3.down);
        if(!Physics.Raycast(ray1, out var hit1, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        float height = hit1.point.y;

        var ray2 = new Ray(new Vector3(bounds.xMax - margin, rayStartHeight, bounds.yMin + margin), Vector3.down);
        if(!Physics.Raycast(ray2, out var hit2, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        if(Mathf.Abs(hit2.point.y - height) > allowedHeightDifferential)
            return false;

        var ray3 = new Ray(new Vector3(bounds.xMin + margin, rayStartHeight, bounds.yMax - margin), Vector3.down);
        if(!Physics.Raycast(ray3, out var hit3, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        if(Mathf.Abs(hit3.point.y - height) > allowedHeightDifferential)
            return false;

        var ray4 = new Ray(new Vector3(bounds.xMax - margin, rayStartHeight, bounds.yMax - margin), Vector3.down);
        if(!Physics.Raycast(ray4, out var hit4, rayLength, LayerMask.GetMask(groundLayer)))
            return false;

        if(Mathf.Abs(hit4.point.y - height) > allowedHeightDifferential)
            return false;

        return true;
    }
}
