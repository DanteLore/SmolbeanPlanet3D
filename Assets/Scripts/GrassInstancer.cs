using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrassInstancer : MonoBehaviour
{
    private class Batch
    {
        public List<Matrix4x4> batchData = new List<Matrix4x4>();
        public Vector3 center;
    }

    private const int BATCH_SIZE = 1024;
    public int instanceAttemptsPerSquareMeter = 1000;
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
    public float renderThreshold = 100f;

    private List<Batch> batches;
    private int rayLayerMask;
    private int groundLayerMask;
    private int groundLayerNum;
    private float xNoiseOffset;
    private float yNoiseOffset;
    private Bounds mapBounds;
    public Transform cameraTransform;

    void Update()
    {
        foreach(var batch in batches)
            if(Vector3.Distance(batch.center, cameraTransform.position) < renderThreshold)
            for(int i = 0; i < mesh.subMeshCount; i++)
                Graphics.DrawMeshInstanced(mesh, i, material, batch.batchData, null, UnityEngine.Rendering.ShadowCastingMode.Off, true);
    }

    void Start()
    {
        Draw();
    }

    public void Draw()
    {
        System.DateTime start = System.DateTime.Now;
        Random.InitState(randomSeed);
        xNoiseOffset = UnityEngine.Random.Range(0f, 1000f);
        yNoiseOffset = UnityEngine.Random.Range(0f, 1000f);

        rayLayerMask = LayerMask.GetMask(occlusionLayers.Append(groundLayer).ToArray());
        groundLayerMask = LayerMask.GetMask(groundLayer);
        groundLayerNum = LayerMask.NameToLayer(groundLayer);

        mapBounds = FindAnyObjectByType<GridManager>().GetMapBounds();
        float area = mapBounds.size.x * mapBounds.size.y;
        int instanceCount = Mathf.CeilToInt(instanceAttemptsPerSquareMeter * area);
        Debug.Log($"Map area {area}sqm => {instanceCount} instance attempts total");

        int desiredBatchCount = instanceCount / BATCH_SIZE;
        float sqrt = Mathf.Sqrt(desiredBatchCount);
        int xSlices = Mathf.FloorToInt(sqrt);
        int zSlices = Mathf.CeilToInt(sqrt);
        int batchCount = xSlices * zSlices;

        Debug.Log($"Dividing the map {xSlices}x{zSlices} == {batchCount} batches == {batchCount * BATCH_SIZE} objects.");

        batches = new List<Batch>();
        int itemAdded = 0;

        foreach(var subBounds in SplitBounds(mapBounds, xSlices, zSlices))
        {
            var currentBatch = new Batch();
            currentBatch.center = subBounds.center;
            batches.Add(currentBatch);

            for(int i = 0; i < BATCH_SIZE; i++)
                if(CreateItemIfPossible(currentBatch.batchData, subBounds))
                    itemAdded++;
        }

        Debug.Log("Actual instances added: " + itemAdded);
        Debug.Log($"Setup grass instance data in {(System.DateTime.Now - start).Seconds}s");
    }

    public IEnumerable<Bounds> SplitBounds(Bounds originalBounds, int xSlices, int zSlices)
    {
        Vector3 size = originalBounds.size;
        Vector3 min = originalBounds.min;

        float subSizeX = size.x / xSlices;
        float subSizeZ = size.z / zSlices;

        for (int i = 0; i < xSlices; i++)
        {
            for (int j = 0; j < zSlices; j++)
            {
                Vector3 subMin = new Vector3(
                    min.x + i * subSizeX,
                    min.y,
                    min.z + j * subSizeZ
                );

                Vector3 subMax = new Vector3(
                    subMin.x + subSizeX,
                    originalBounds.max.y,
                    subMin.z + subSizeZ
                );

                Bounds subBounds = new Bounds(
                    subMin + 0.5f * (subMax - subMin),
                    subMax - subMin
                );
                
                yield return subBounds;
            }
        }
    }


    private bool CreateItemIfPossible(List<Matrix4x4> batch, Bounds subBounds)
    {
        float posX = Random.Range(subBounds.min.x, subBounds.max.x);
        float posZ = Random.Range(subBounds.min.z, subBounds.max.z);

        Ray ray = new Ray(new Vector3(posX, 100f, posZ), Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, 200f, rayLayerMask))
            return false;

        var posY = hit.point.y;
        if (hit.transform.gameObject.layer != groundLayerNum || posY < minHeight)
            return false;

        float angle = Vector3.Angle(Vector3.up, hit.normal);
        if (angle > maxSlopeAngle)
            return false;

        float noise = Mathf.PerlinNoise((posX + xNoiseOffset) / (mapBounds.size.x * noiseScale), (posZ + yNoiseOffset) / (mapBounds.size.z * noiseScale));
        if(Random.Range(0.1f, 1.0f) > noise)
            return false;

        var pos = new Vector3(posX, posY, posZ);
        var rot = new Vector3(Random.Range(-maxTilt, maxTilt), Random.Range(0f, 360f), Random.Range(-maxTilt, maxTilt));
        float s = Mathf.Lerp(minScale, maxScale, noise);
        var scale = new Vector3(s, s, s);

        var matrix = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scale);
        batch.Add(matrix);
        return true;
    }
}
