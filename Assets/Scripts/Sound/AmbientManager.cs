using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    public float movementResolutionMeters = 4f;
    public int mapSquareRadius = 4;
    public string groundLayer = "Ground";
    public string seaLayer = "Sea";

    private GridManager gridManager;
    private SoundPlayer soundPlayer;
    private Vector3 lastPosition;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        soundPlayer = GetComponent<SoundPlayer>();
        soundPlayer.Play("Wind");
        soundPlayer.Play("Waves");

        lastPosition = transform.position;
        UpdateVolumes(lastPosition);
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if(Vector3.SqrMagnitude(lastPosition - pos) > movementResolutionMeters * movementResolutionMeters)
        {
            lastPosition = pos;
            UpdateVolumes(pos);
        }
    }

    private void UpdateVolumes(Vector3 pos)
    {
        if (Physics.Raycast(new Ray(pos, Vector3.down), out var hit, float.MaxValue, LayerMask.GetMask(groundLayer, seaLayer)))
        {
            Vector2Int square = gridManager.GetGameSquareFromWorldCoords(hit.point);

            int landCount = 0;
            int seaCount = 0;

            int minX = Mathf.Max(0, square.x - mapSquareRadius);
            int maxX = Mathf.Min(gridManager.GameMapWidth, square.x + mapSquareRadius);
            int minY = Mathf.Max(0, square.y - mapSquareRadius);
            int maxY = Mathf.Min(gridManager.GameMapHeight, square.y + mapSquareRadius);

            for(int y = minY; y < maxY; y++)
            {
                for(int x = minX; x < maxX; x++)
                {
                    if (gridManager.GameMap[y * gridManager.GameMapWidth + x] <= 0)
                        seaCount++;
                    else
                        landCount++;
                }  
            }

            float seaVolume = (1.0f * seaCount) / (seaCount + landCount);
            float landVolume = (1.0f * landCount) / (seaCount + landCount);

            soundPlayer.SetVolume("Waves", seaVolume);
            soundPlayer.SetVolume("Wind", landVolume);
        }
    }
}
