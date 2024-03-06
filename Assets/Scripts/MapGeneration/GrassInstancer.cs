using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Profiling;
using System.Collections;

public class GrassInstancer : MonoBehaviour, IObjectGenerator
{
    private class Batch
    {
        public List<Matrix4x4> batchData = new ();
        public Bounds bounds;
    }

    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

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
    private int occlusionLayerMask;
    private int groundLayerMask;
    private int groundLayerNum;
    private float xNoiseOffset;
    private float yNoiseOffset;
    private Bounds mapBounds;
    public Camera mainCamera;
    private GridManager gridManager;
    private float renderThresholdSqr;

    void Update()
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);
        var cameraPos = mainCamera.transform.position;

        foreach(var batch in batches)
        {
            Vector3 p = batch.bounds.ClosestPoint(cameraPos);

            if(Vector3.SqrMagnitude(p - cameraPos) < renderThresholdSqr && GeometryUtility.TestPlanesAABB(planes, batch.bounds))
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
        gridManager = FindFirstObjectByType<GridManager>();
        renderThresholdSqr = renderThreshold * renderThreshold;
        StartCoroutine(GenerateGrass());
    }

    public void Clear()
    {
        batches = new List<Batch>();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        yield return GenerateGrass();
    }

    public IEnumerator Load(SaveFileData data)
    {
        yield return GenerateGrass();
    }

    private IEnumerator GenerateGrass()
    {
        mapBounds = gridManager.GetIslandBounds();
        batches = new List<Batch>();

        //System.DateTime start = System.DateTime.Now;
        Random.InitState(randomSeed);
        xNoiseOffset = Random.Range(0f, 1000f);
        yNoiseOffset = Random.Range(0f, 1000f);

        occlusionLayerMask = LayerMask.GetMask(occlusionLayers.ToArray());
        groundLayerNum = LayerMask.NameToLayer(groundLayer);
        groundLayerMask = LayerMask.GetMask(groundLayer);

        List<Vector3> grassBlades = new();
        yield return GenerateGrassBlades(grassBlades);

        //Debug.Log($"Created {grassBlades.Count} grass blades");

        yield return null;

        // Batch collapse performance
        // No batch collapse:       ~5.3ms      (Fixed batch grid based on BATCH_SIZE and number of blades per m2)
        // CollapseAdjacentBatches: ~4.0ms      (The old way - create a grid of batches then collapse them together RLE style)
        // CreateBatchesQuad:       ~5.0ms      (Getting some very small batches this way...)
        //     Binary tree:         ~4.3ms      (Binary split, alternating horizontal and vertical as you recurse)
        //     Plus min size:       ~3.5ms      (As above but impose a minimum batch size - dropping extra grass if splitting would cause small batches)

        batches = CreateBatchesBinarySplit(mapBounds, grassBlades);

        //Debug.Log("Batches created by quad tree: " + batches.Count());
        //Debug.Log(string.Join(",", batches.Select(b => b.batchData.Count)));
        //Debug.Log($"Setup grass instance data in {(System.DateTime.Now - start).Seconds}s");

        yield return null;
    }

    private IEnumerator GenerateGrassBlades(List<Vector3> grassBlades)
    {
        Vector3 size = mapBounds.size;
        float tileArea = gridManager.tileSize * gridManager.tileSize;

        foreach (var meshRenderer in gridManager.GetAllGroundMeshes())
        {
            Bounds bounds = meshRenderer.bounds;

            int instanceCount = Mathf.CeilToInt(tileArea * instanceAttemptsPerSquareMeter);

            for (int i = 0; i < instanceCount; i++)
            {
                if (TryCreateGrassBlade(bounds.min, bounds.max, size, out Vector3 pos))
                {
                    grassBlades.Add(pos);
                }
            }
        }
        return null;
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

    private bool TryCreateGrassBlade(Vector3 min, Vector3 max, Vector3 size, out Vector3 pos)
    {
        float posX = Random.Range(min.x, max.x);
        float posZ = Random.Range(min.z, max.z);
        pos = Vector3.zero;

        float noise = Mathf.PerlinNoise((posX + xNoiseOffset) / (size.x * noiseScale), (posZ + yNoiseOffset) / (size.z * noiseScale));
        if (Random.Range(0.1f, 1.0f) > noise)
            return false;

        Ray ray = new (new Vector3(posX, 1000f, posZ), Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit groundHit, float.PositiveInfinity, groundLayerMask))
            return false;

        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        if (angle > maxSlopeAngle)
            return false;

        var posY = groundHit.point.y;
        if (posY < minHeight)
            return false;

        if (Physics.Raycast(ray, out RaycastHit _, float.PositiveInfinity, occlusionLayerMask))
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
}
