using System;
using UnityEngine;

public class AmbientManager : MonoBehaviour
{
    public float movementResolutionMeters = 4f;
    public float minimumUpdateTimeSeconds = 1f;
    public int mapSquareRadius = 4;
    public float coastHyperbolicWeight = 4f;
    [Range(0, 1)]
    public float coastWindVolumeDip = 0.5f;
    public string groundLayer = "Ground";
    public string seaLayer = "Sea";

    private GridManager gridManager;
    private SoundPlayer soundPlayer;
    private Vector3 lastUpdatePosition;
    private float lastUpdateTime;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        soundPlayer = GetComponent<SoundPlayer>();
        soundPlayer.Play("Wind");
        soundPlayer.Play("Waves");

        lastUpdatePosition = transform.position;
        lastUpdateTime = Time.time;
        UpdateVolumes(lastUpdatePosition);
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if(Vector3.SqrMagnitude(lastUpdatePosition - pos) > movementResolutionMeters * movementResolutionMeters
            || Time.time - lastUpdateTime >= minimumUpdateTimeSeconds)
        {
            lastUpdateTime = Time.time;
            lastUpdatePosition = pos;
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

            float coastVolume = Hyperbolic(1 - Mathf.Abs(seaVolume - landVolume));

            float windVolume = 1 - coastVolume * coastWindVolumeDip;

            soundPlayer.SetVolume("Waves", coastVolume);
            soundPlayer.SetVolume("Wind", windVolume);
        }
    }

    private float Hyperbolic(float x)
    {
        float n = coastHyperbolicWeight;
        return (n + 1) * x / ((n * x) + 1);
    }
}
