using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

// Batch collapse performance
// No batch collapse:       ~5.3ms      (Fixed batch grid based on BATCH_SIZE and number of blades per m2)
// CollapseAdjacentBatches: ~4.0ms      (The old way - create a grid of batches then collapse them together RLE style)
// CreateBatchesQuad:       ~5.0ms      (Getting some very small batches this way...)
//     Binary tree:         ~4.3ms      (Binary split, alternating horizontal and vertical as you recurse)
//     Plus min size:       ~3.5ms      (As above but impose a minimum batch size - dropping extra grass if splitting would cause small batches)

// New grass system using fewer grass blades and improved billboard shader
// Also using Profile Analyzer in unity to get average times
// Note that deep profiler is attached but not recording, so numbers are high!
//                          Gen:        Frame Mean:     Frame Max:
// First run:               14.1s       2.7ms           4.26ms
// Removed Linq:            13.5s       2.47ms          2.96ms
// foreach to for:          10.9s       2.4ms           2.57ms
// Visible batch list:      ~10s        2.0ms           2.16ms
// Culling Group:           10.9s       2.18ms          2.47ms   (Note: had to move the camera to hit the culling code!)
// Camera deadzone:         11s         2.15ms          2.64ms   (Note: movement makes this inaccurate probs)
// Without deep profile:    4.2s        1.74ms          2.16ms   (Just for info)

// ChatGPT recommended using a Morton function and a sorted list of grass to get 100% batches.  It makes setup MUCH slower
// And delivers only small gains in performance
// Could be because small batches are not being thrown away, so more grass. Plus batch overlaps
//                          Gen:        Frame Mean:     Frame Max:
// Morton:                              2.11ms          2.82ms   (With movement)
// Morton:                  46.3s (!)   1.97ms          2.18ms   (No movement, just to make it fair!)
// No profiler:             16.4s       1.62ms          1.80ms
// Prefetch submech count:  16.4s       1.63ms          1.87ms   (Too close to call!)
// Batches to array         16.5s       1.63ms          1.89ms   (Hmmm)

public class GrassInstancer : MonoBehaviour, IObjectGenerator
{
    private class Batch
    {
        public List<Matrix4x4> batchData = new();
        public Bounds bounds;
    }

    public enum BatchGenerationAlgorithm { Morton, BTree }

    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

    private const int BATCH_SIZE = 1023;
    public int instanceAttemptsPerSquareMeter = 1000;
    public int minInstancesForSplit = 1536;
    public int minBatchSize = 256;
    public Mesh mesh;
    public Material material;
    public Texture2D wearTexture;
    public float mapWidth = 400f;
    public float mapHeight = 400f;
    public float mapOffsetX = -200f;
    public float mapOffsetY = -200f;
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
    public AnimationCurve grassWeightCurve;
    public float positionThreshold = 0.5f;
    public float rotationThreshold = 2.0f;
    public BatchGenerationAlgorithm generationAlgorithm = BatchGenerationAlgorithm.BTree;

    private Batch[] batches;
    private int occlusionLayerMask;
    private int groundLayerMask;
    private float xNoiseOffset;
    private float yNoiseOffset;
    private Bounds mapBounds;
    public Camera mainCamera;
    private GridManager gridManager;
    private float renderThresholdSqr;
    private float positionThresholdSqr;

    private readonly Plane[] planes = new Plane[6];
    private Vector3 lastCamPos;
    private Quaternion lastCamRot;
    private Transform cameraTransform;
    private int subMeshCount;
    private CullingGroup cullingGroup;
    private int grassBladeCount;
    private readonly List<Batch> visibleBatches = new();

    private void Update()
    {
        cameraTransform.GetPositionAndRotation(out Vector3 camPos, out Quaternion camRot);

        float movedSqr = (camPos - lastCamPos).sqrMagnitude;
        float angleDiff = Quaternion.Angle(camRot, lastCamRot);

        if (movedSqr > positionThresholdSqr || angleDiff > rotationThreshold)
        {
            GeometryUtility.CalculateFrustumPlanes(mainCamera, planes);
            lastCamPos = cameraTransform.position;
            lastCamRot = cameraTransform.rotation;

            // Calculate visible batches
            visibleBatches.Clear();
            for (int batchIndex = 0; batchIndex < batches.Length; batchIndex++)
            {
                Batch batch = batches[batchIndex];
                //if (batch.bounds.SqrDistance(lastCamPos) < renderThresholdSqr && GeometryUtility.TestPlanesAABB(planes, batch.bounds))
                if (batch.bounds.SqrDistance(lastCamPos) < renderThresholdSqr && cullingGroup.IsVisible(batchIndex))
                {
                    visibleBatches.Add(batch);
                }
            }
        }

        // Render the visible batches
        for (int batchIndex = 0; batchIndex < visibleBatches.Count; batchIndex++)
        {
            Batch batch = visibleBatches[batchIndex];

            for (int subMeshIndex = 0; subMeshIndex < subMeshCount; subMeshIndex++)
            {
                Graphics.DrawMeshInstanced(mesh, subMeshIndex, material, batch.batchData, null, UnityEngine.Rendering.ShadowCastingMode.Off, true);
            }
        }
    }

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        renderThresholdSqr = renderThreshold * renderThreshold;
        positionThresholdSqr = positionThreshold * positionThreshold;
        cameraTransform = mainCamera.transform;
        subMeshCount = mesh.subMeshCount;
        batches = Array.Empty<Batch>();

        StartCoroutine(GenerateGrass());
    }

    public void Clear()
    {
        batches = Array.Empty<Batch>();

        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
    }

    public void OnDestroy()
    {
        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        yield return GenerateGrass();
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        yield return GenerateGrass();
    }

    private IEnumerator GenerateGrass()
    {
        mapBounds = gridManager.GetIslandBounds();

        Random.InitState(randomSeed);
        xNoiseOffset = Random.Range(0f, 1000f);
        yNoiseOffset = Random.Range(0f, 1000f);

        occlusionLayerMask = LayerMask.GetMask(occlusionLayers);
        groundLayerMask = LayerMask.GetMask(groundLayer);

        List<Vector3> grassBlades = new();
        yield return GenerateGrassBlades(grassBlades);
        grassBladeCount = grassBlades.Count;

        yield return null;

        if (generationAlgorithm == BatchGenerationAlgorithm.Morton)
            batches = CreateBatchesMorton(mapBounds, grassBlades).ToArray();
        else
            batches = CreateBatchesBinarySplit(mapBounds, grassBlades).ToArray();

        //Debug.Log("Batches created: " + batches.Count);
        //Debug.Log("Batches less than 100% full: " + batches.Count(b => b.batchData.Count < BATCH_SIZE));

        yield return null;

        SetupCullingGroup();

        yield return null;
    }

    private void SetupCullingGroup()
    {
        if (cullingGroup != null)
        {
            cullingGroup.Dispose();
            cullingGroup = null;
        }

        var allSpheres = new BoundingSphere[batches.Length];
        for (int i = 0; i < batches.Length; i++)
        {
            var bounds = batches[i].bounds;
            allSpheres[i] = new BoundingSphere(bounds.center, bounds.extents.magnitude);
        }

        cullingGroup = new CullingGroup();
        cullingGroup.targetCamera = mainCamera;
        cullingGroup.SetBoundingSpheres(allSpheres);
        cullingGroup.SetBoundingSphereCount(allSpheres.Length);
    }

    private IEnumerator GenerateGrassBlades(List<Vector3> grassBlades)
    {
        float tileArea = gridManager.tileSize * gridManager.tileSize;
        Color[] groundTextureData = wearTexture.GetPixels();

        foreach (var meshRenderer in gridManager.GetAllGroundMeshes())
        {
            Bounds bounds = meshRenderer.bounds;

            int instanceCount = Mathf.CeilToInt(tileArea * instanceAttemptsPerSquareMeter);

            for (int i = 0; i < instanceCount; i++)
            {
                if (TryCreateGrassBlade(bounds.min, bounds.max, groundTextureData, out Vector3 pos))
                {
                    grassBlades.Add(pos);
                }
            }
        }
        return null;
    }

    private List<Batch> CreateBatchesMorton(Bounds mapBounds, List<Vector3> grassBlades)
    {
        var batches = new List<Batch>();

        grassBlades.Sort((a, b) => Morton(a, mapBounds).CompareTo(Morton(b, mapBounds)));
        for (int i = 0; i < grassBlades.Count; i += BATCH_SIZE)
        {
            var chunk = grassBlades.GetRange(i, Mathf.Min(BATCH_SIZE, grassBlades.Count - i));
            var bounds = ComputeBounds(chunk);
            batches.Add(CreateBatch(bounds, chunk));
        }

        return batches;
    }

    private List<Batch> CreateBatchesBinarySplit(Bounds bounds, List<Vector3> grassBlades, bool splitHorizontal = true)
    {
        var result = new List<Batch>();
        int grassCount = grassBlades.Count;

        if (grassCount <= BATCH_SIZE && grassCount >= minBatchSize)
        {
            result.Add(CreateBatch(bounds, grassBlades));
        }
        else if (grassCount <= minInstancesForSplit)
        {
            // Splitting would cause unacceptably small child batches - better to just dump the extra grass
            grassBlades.RemoveRange(BATCH_SIZE, grassBlades.Count - BATCH_SIZE);
            result.Add(CreateBatch(bounds, grassBlades));
        }
        else // Split this batch into two and recurse!
        {
            var splitBatchBoundsList = splitHorizontal ? SplitBounds(bounds, 2, 1) : SplitBounds(bounds, 1, 2);

            foreach (var batchBounds in splitBatchBoundsList)
            {
                List<Vector3> quadGrass = new();

                for (int i = 0; i < grassBlades.Count; i++)
                {
                    if (batchBounds.Contains(grassBlades[i]))
                    {
                        quadGrass.Add(grassBlades[i]);
                    }
                }

                if (quadGrass.Count >= minBatchSize)
                {
                    var childBatches = CreateBatchesBinarySplit(batchBounds, quadGrass, !splitHorizontal);
                    result.AddRange(childBatches);
                }
            }
        }

        return result;
    }

    public static Bounds ComputeBounds(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds b = new Bounds(points[0], Vector3.zero);

        for (int i = 1; i < points.Count; i++)
            b.Encapsulate(points[i]);

        return b;
    }

    // Computes a 32-bit Morton code (Z-order) from a world-space position.
    // OK, my AI friend helped me with this!  I did not know what a Morton hash was!
    public static uint Morton(Vector3 pos, Bounds bounds)
    {
        float nx = Mathf.InverseLerp(bounds.min.x, bounds.max.x, pos.x);
        float nz = Mathf.InverseLerp(bounds.min.z, bounds.max.z, pos.z);

        // Quantize to 16 bits
        uint ix = (uint)(Mathf.Clamp01(nx) * 65535f);
        uint iz = (uint)(Mathf.Clamp01(nz) * 65535f);

        // Interleave and return
        return (SpreadBits(iz) << 1) | SpreadBits(ix);
    }

    // Spreads out the lower 16 bits of x so that there are zeroes between each bit:
    // e.g. fed:  abcdefghijklmnop  ->  a0b0c0d0e0f0g0h0i0j0k0l0m0n0o0p0
    private static uint SpreadBits(uint x)
    {
        x &= 0x0000FFFF;  // keep only lower 16 bits
        x = (x | (x << 8)) & 0x00FF00FF;
        x = (x | (x << 4)) & 0x0F0F0F0F;
        x = (x | (x << 2)) & 0x33333333;
        x = (x | (x << 1)) & 0x55555555;
        return x;
    }

    private Batch CreateBatch(Bounds bounds, List<Vector3> grassBlades)
    {
        Batch batch = new();

        for (int i = 0; i < grassBlades.Count; i++)
            batch.batchData.Add(GenerateGrassData(grassBlades[i]));

        batch.bounds = bounds;

        return batch;
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

    private bool TryCreateGrassBlade(Vector3 min, Vector3 max, Color[] groundTextureData, out Vector3 pos)
    {
        pos = Vector3.zero;

        float posX = Random.Range(min.x, max.x);
        float posZ = Random.Range(min.z, max.z);

        float mapX = (posX - mapOffsetX) / mapWidth;
        float mapZ = (posZ - mapOffsetY) / mapHeight;

        int texX = Mathf.CeilToInt(wearTexture.width * mapX);
        int texY = Mathf.CeilToInt(wearTexture.height * mapZ);

        float noise = groundTextureData[texY * wearTexture.width + texX].g;
        noise = grassWeightCurve.Evaluate(noise);

        if (Random.Range(0.0f, 1.0f) > noise)
            return false;

        Ray ray = new(new Vector3(posX, 10000f, posZ), Vector3.down);
        if (!Physics.Raycast(ray, out var groundHit, float.PositiveInfinity, groundLayerMask))
            return false;

        float angle = Vector3.Angle(Vector3.up, groundHit.normal);
        if (angle > maxSlopeAngle)
            return false;

        var posY = groundHit.point.y;
        if (posY < minHeight)
            return false;

        if (Physics.Raycast(ray, out var _, float.PositiveInfinity, occlusionLayerMask))
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
                Vector3 subMin = new(
                    min.x + i * subSizeX,
                    min.y,
                    min.z + j * subSizeZ
                );

                Vector3 subMax = new(
                    subMin.x + subSizeX,
                    originalBounds.max.y,
                    subMin.z + subSizeZ
                );

                Bounds subBounds = new(
                    subMin + 0.5f * (subMax - subMin),
                    subMax - subMin
                );

                yield return subBounds;
            }
        }
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        // Nothing to save here - easier to regenerate grass - and nobody's counting :D
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Handles.ShouldRenderGizmos())
            return;

        if (batches == null)
            return;

        foreach (var batch in batches)
        {
            Gizmos.color = visibleBatches.Contains(batch) ? Color.yellow : Color.red;
            Gizmos.DrawWireCube(batch.bounds.center, batch.bounds.size);
        }

        DisplayBatchData();
    }

    private void DisplayBatchData()
    {
        if (batches == null || batches.Length == 0)
            return;

        int totalBatches = batches.Length;
        int totalVisible = visibleBatches.Count;

        var sizes = batches
            .Select(b => b.batchData.Count)
            .OrderBy(x => x)
            .ToArray();

        int min = sizes.First();
        int max = sizes.Last();
        float mean = (float)sizes.Average();
        int median = sizes[sizes.Length / 2];

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            normal = { textColor = Color.white }
        };

        string stats =
            $"Blades of grass: {grassBladeCount}\n" +
            $"Grass Batches: {totalBatches}\n" +
            $"Visible:       {totalVisible}\n\n" +
            $"Size  Min: {min}\n" +
            $"      Max: {max}\n" +
            $"      Mean: {mean:F1}\n" +
            $"      Median: {median}";

        var rect = new Rect(10, 110, 200, 300);
        Handles.BeginGUI();
        GUI.Box(rect, GUIContent.none);
        GUI.Label(rect, stats, style);
        Handles.EndGUI();
    }
#endif

}
