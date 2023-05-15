using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Instancer : MonoBehaviour
{
    public int instanceCount;
    public Mesh mesh;
    public Material material;
    public string groundLayer = "Ground";
    public string[] occlusionLayers = { "Nature" };
    public float maxSlopeAngle = 45f;
    public float minHeight = -0.2f;
    public int randomSeed = 1234;
    public float minScale = 0.6f;
    public float maxScale = 1.5f;
    public float maxTilt = 10f;
    public float noiseScale = 0.1f;

    private List<List<Matrix4x4>> batches;
    private Bounds bounds;
    private int rayLayerMask;
    private int groundLayerMask;
    private int groundLayerNum;
    private float xOffset;
    private float yOffset;

    void Update()
    {
        foreach(var batch in batches)
            for(int i = 0; i < mesh.subMeshCount; i++)
                Graphics.DrawMeshInstanced(mesh, i, material, batch);
    }

    void Start()
    {
        Random.InitState(randomSeed);

        batches = new List<List<Matrix4x4>>();
        bounds = FindAnyObjectByType<GridManager>().GetMapBounds();
        rayLayerMask = LayerMask.GetMask(occlusionLayers.Append(groundLayer).ToArray());
        groundLayerMask = LayerMask.GetMask(groundLayer);
        groundLayerNum = LayerMask.NameToLayer(groundLayer);

        xOffset = UnityEngine.Random.Range(0f, 1000f);
        yOffset = UnityEngine.Random.Range(0f, 1000f);

        int itemsCreated = 0;
        int itemsAddedToCurrentBatch = 0;
        batches.Add(new List<Matrix4x4>());
        while(itemsCreated < instanceCount)
        {
            if(itemsAddedToCurrentBatch >= 1000)
            {
                batches.Add(new List<Matrix4x4>());
                itemsAddedToCurrentBatch = 0;
            }

            if(CreateItemIfPossible(batches.Last()))
            {
                itemsAddedToCurrentBatch++;
                itemsCreated++;
            }
        }

        Debug.Log("Number of grass batches: " + batches.Count);
    }

    private bool CreateItemIfPossible(List<Matrix4x4> batch)
    {
        float posX = Random.Range(bounds.min.x, bounds.max.x);
        float posZ = Random.Range(bounds.min.z, bounds.max.z);

        Ray ray = new Ray(new Vector3(posX, 100f, posZ), Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, rayLayerMask))
            return false;

        var posY = hit.point.y;
        if (hit.transform.gameObject.layer != groundLayerNum || posY < minHeight)
            return false;

        float angle = Vector3.Angle(Vector3.up, hit.normal);
        if (angle > maxSlopeAngle)
            return false;

        float sample = Mathf.PerlinNoise((posX + xOffset) / (bounds.size.x * noiseScale), (posZ + yOffset) / (bounds.size.z * noiseScale));
        if(Random.Range(0.1f, 1.0f) > sample)
            return false;

        var pos = new Vector3(posX, posY, posZ);
        var rot = new Vector3(Random.Range(-maxTilt, maxTilt), Random.Range(-90f, 90f), Random.Range(-maxTilt, maxTilt));
        float s = Random.Range(minScale, maxScale);
        var scale = new Vector3(s, s, s);

        var matrix = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scale);
        batch.Add(matrix);
        return true;
    }
}
