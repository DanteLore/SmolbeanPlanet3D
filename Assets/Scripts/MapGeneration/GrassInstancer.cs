using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GrassInstancer : MonoBehaviour, IObjectGenerator
{
    private class Batch
    {
        public List<Matrix4x4> batchData = new List<Matrix4x4>();
        public Bounds bounds;
    }

    public int Priority { get { return 100; } }

    private const int BATCH_SIZE = 1024;
    public int instanceAttemptsPerSquareMeter = 1000;
    public int minInstancesForSplit = 1536;
    public int minBatchSize = 256;
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
    public Camera mainCamera;
    private float renderThresholdSqr;

    void Update()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

        foreach(var batch in batches)
        {
            Vector3 p = batch.bounds.ClosestPoint(mainCamera.transform.position);

            if(Vector3.SqrMagnitude(p - mainCamera.transform.position) < renderThresholdSqr && GeometryUtility.TestPlanesAABB(planes, batch.bounds))
            {
                for(int i = 0; i < mesh.subMeshCount; i++)
                {
                    Graphics.DrawMeshInstanced(mesh, i, material, batch.batchData, null, UnityEngine.Rendering.ShadowCastingMode.Off, true);
                }
            }
        }
    }

    void Start()
    {
        renderThresholdSqr = renderThreshold * renderThreshold;
        GenerateGrass();
    }

    public void Clear()
    {
        batches = new List<Batch>();
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        GenerateGrass();
    }

    private void GenerateGrass()
    {
        System.DateTime start = System.DateTime.Now;
        Random.InitState(randomSeed);
        xNoiseOffset = UnityEngine.Random.Range(0f, 1000f);
        yNoiseOffset = UnityEngine.Random.Range(0f, 1000f);

        rayLayerMask = LayerMask.GetMask(occlusionLayers.Append(groundLayer).ToArray());
        groundLayerMask = LayerMask.GetMask(groundLayer);
        groundLayerNum = LayerMask.NameToLayer(groundLayer);

        mapBounds = FindAnyObjectByType<GridManager>().GetIslandBounds();
        float area = mapBounds.size.x * mapBounds.size.y;
        int instanceCount = Mathf.CeilToInt(instanceAttemptsPerSquareMeter * area);
        Debug.Log($"Map area {area}sqm => {instanceCount} instance attempts total");
        
        List<Vector3> grassBlades = new List<Vector3>();

        for(int i = 0; i < instanceCount; i++)
        {
            if(TryCreateGrassBlade(mapBounds, out Vector3 pos))
            {
                grassBlades.Add(pos);
            }
        }

        Debug.Log($"Actually created {grassBlades.Count} grass blades");

        // Batch collapse performance
        // No batch collapse:       ~5.3ms      (Fixed batch grid based on BATCH_SIZE and number of blades per m2)
        // CollapseAdjacentBatches: ~4.0ms      (The old way - create a grid of batches then collapse them together RLE style)
        // CreateBatchesQuad:       ~5.0ms      (Getting some very small batches this way...)
        //     Binary tree:         ~4.3ms      (Binary split, alternating horizontal and vertical as you recurse)
        //     Plus min size:       ~3.5ms      (As above but impose a minimum batch size - dropping extra grass if splitting would cause small batches)

        batches = CreateBatchesBinarySplit(mapBounds, grassBlades);

        Debug.Log("Batches created by quad tree: " + batches.Count());
        Debug.Log(string.Join(",", batches.Select(b => b.batchData.Count)));
        Debug.Log($"Setup grass instance data in {(System.DateTime.Now - start).Seconds}s");
    }

    private List<Batch> CreateBatchesBinarySplit(Bounds bounds, List<Vector3> grassBlades, bool splitHorizontal = true)
    {
        var result = new List<Batch>();
        int grassCount = grassBlades.Count;

        if(grassCount <= BATCH_SIZE && grassCount >= minBatchSize)
        {
            result.Add(CreateBatch(bounds, grassBlades));
        }
        else if(grassCount <= minInstancesForSplit)
        {
            // Splitting would cause unacceptably small child batches - better to just dump the extra grass
            result.Add(CreateBatch(bounds, grassBlades.Take(BATCH_SIZE)));
        }
        else // Split this batch into two and recurse!
        {
            var splitBatches = splitHorizontal ? SplitBounds(bounds, 2, 1) : SplitBounds(bounds, 1, 2);

            foreach(var splitBatch in splitBatches)
            {
                var quadGrass = grassBlades.Where(g => splitBatch.Contains(g)).ToList();
                if(quadGrass.Count >= minBatchSize)
                {
                    var childBatches = CreateBatchesBinarySplit(splitBatch, quadGrass, !splitHorizontal);
                    result.AddRange(childBatches);
                }
            }
        }

        return result;
    }

    private Batch CreateBatch(Bounds bounds, IEnumerable<Vector3> grassBlades)
    {
        return new Batch
        {
            batchData = grassBlades.Select(GenerateGrassData).ToList(),
            bounds = bounds
        };
    }

    private Matrix4x4 GenerateGrassData(Vector3 pos)
    {
        float noise = Mathf.PerlinNoise((pos.x + xNoiseOffset) / (mapBounds.size.x * noiseScale), (pos.z + yNoiseOffset) / (mapBounds.size.z * noiseScale));
        var rot = new Vector3(Random.Range(-maxTilt, maxTilt), Random.Range(0f, 360f), Random.Range(-maxTilt, maxTilt));
        float s = Mathf.Lerp(minScale, maxScale, noise);
        var scale = new Vector3(s, s, s);

        var matrix = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scale);
        return matrix;
    }

    private bool TryCreateGrassBlade(Bounds bounds, out Vector3 pos)
    {
        float posX = Random.Range(bounds.min.x, bounds.max.x);
        float posZ = Random.Range(bounds.min.z, bounds.max.z);
        pos = Vector3.zero;

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

        pos = new Vector3(posX, posY, posZ);
        return true;
    }

    private IEnumerable<Bounds> SplitBounds(Bounds originalBounds, int xSlices, int zSlices)
    {
        Vector3 size = originalBounds.size;
        Vector3 min = originalBounds.min;

        float subSizeX = size.x / xSlices;
        float subSizeZ = size.z / zSlices;

        for (int j = 0; j < zSlices; j++)
        {
            for (int i = 0; i < xSlices; i++)
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

//****************************
/*


    private void TheOldWay(System.DateTime start, int instanceCount)
    {
        // Basically take the optimistic view that we can place grass everywhere we want to, and never want to go over the 1024 batch size limit.
        // In reality this doesn't happen very often - but we can't predict that, so aim to collapse batches at the end.
        int desiredBatchCount = instanceCount / BATCH_SIZE;
        float sqrt = Mathf.Sqrt(desiredBatchCount);
        int xSlices = Mathf.FloorToInt(sqrt);
        int zSlices = Mathf.CeilToInt(sqrt);
        int batchCount = xSlices * zSlices;

        Debug.Log($"Dividing the map {xSlices}x{zSlices} == {batchCount} batches == {batchCount * BATCH_SIZE} objects.");

        batches = new List<Batch>();
        int itemAdded = 0;

        foreach (var subBounds in SplitBounds(mapBounds, xSlices, zSlices))
        {
            var currentBatch = new Batch();
            currentBatch.bounds = subBounds;
            batches.Add(currentBatch);

            for (int i = 0; i < BATCH_SIZE; i++)
                if (CreateItemIfPossible(currentBatch.batchData, subBounds))
                    itemAdded++;
        }

        Debug.Log("Actual instances added: " + itemAdded);
        Debug.Log("Batches created: " + batches.Count());

        batches = batches.Where(b => b.batchData.Count() >= BATCH_SIZE / 20).ToList();

        Debug.Log("Batches kept (large enough): " + batches.Count());
        Debug.Log("Average batch size pre-merge (max 1024): " + itemAdded / batches.Count());


        batches = CollapseAdjacentBatchesQuadStyle(batches);

        Debug.Log("Batches after merge: " + batches.Count());
        Debug.Log("Average batch size post-merge (max 1024): " + itemAdded / batches.Count());
        Debug.Log($"Setup grass instance data in {(System.DateTime.Now - start).Seconds}s");
    }

    private List<Batch> CollapseAdjacentBatchesQuadStyle(List<Batch> batches)
    {
        return batches;
    }

    private List<Batch> CollapseAdjacentBatches(List<Batch> batches)
    {
        List<Batch> result = new List<Batch>();

        if(batches.Count() <= 1)
            return result;

        int i = 0;
        while(i < batches.Count - 1)
        {
            var current = batches[i];

            int j = i + 1;
            while(
                    j < batches.Count
                    && AreAdjacent(current, batches[j]) 
                    && current.batchData.Count + batches[j].batchData.Count <= BATCH_SIZE 
                )
            {
                current = MergeBatches(current, batches[j]);
                j++;
            }
        
            result.Add(current);
            i = j;
        }

        return result;
    }

    private Batch MergeBatches(Batch b1, Batch b2)
    {
        var newBounds = b1.bounds;
        newBounds.Encapsulate(b2.bounds);

        var newData = b1.batchData.Concat(b2.batchData).ToList();

        return new Batch { batchData = newData, bounds = newBounds };
    }

    private bool AreAdjacent(Batch current, Batch next)
    {
        var p1 = current.bounds.ClosestPoint(next.bounds.center);
        var p2 = next.bounds.ClosestPoint(p1);

        return Vector3.SqrMagnitude(p1 - p2) < 1f;
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
    //*/
}
