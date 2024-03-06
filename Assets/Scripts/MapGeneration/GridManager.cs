using UnityEngine;
using System.Linq;
using Unity.AI.Navigation;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Collections;
using System.Threading;

public class GridManager : MonoBehaviour, IObjectGenerator
{
    public TerrainData terrainData;
    public Material meshMaterial;
    public float fuzzyEdgeFactor = 0.01f;
    public float tileSize = 4.0f;

    public string groundLayer = "Ground";
    public string seaLayer = "Sea";

    public bool addMeshDebugGizmos = false;

    public List<int> GameMap { get; private set; }
    public int GameMapWidth { get; private set; }
    public int GameMapHeight { get; private set; }
    public int MaxLevelNumber { get; private set; }
    public int DrawMapWidth { get; private set; }
    public int DrawMapHeight { get; private set; }

    private MeshData[] map;

    private GameObject Ground { get { return transform.Find("Ground").gameObject; }}
    private GameObject Seabed { get { return transform.Find("Seabed").gameObject; }}

    public int Priority { get { return 0; } }

    public bool NewGameOnly { get { return false; } }

    public bool RunModeOnly { get { return false; } }

    public void Clear()
    {
        ClearMap();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        GameMap = gameMap;
        GameMapWidth = gameMapWidth;
        GameMapHeight = gameMapHeight;
        DrawMapWidth = gameMapWidth + 1;
        DrawMapHeight = gameMapHeight + 1;

        var meshData = terrainData.meshData.ToList();
        var neighbourData = terrainData.neighbourData.ToDictionary(nd => nd.id, nd => nd);

        yield return null;

        var wfc = new WaveFunctionCollapse(gameMapWidth, gameMapHeight, meshData, neighbourData);
        wfc.GenerateMap(gameMap);
        yield return null;

        map = wfc.tiles;

        yield return DrawMap();

        UpdateNavMesh();

        yield return null;
    }

    private void UpdateNavMesh()
    {
        var surface = Ground.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    private IEnumerator DrawMap()
    {
        Vector3 offset = new(DrawMapWidth * tileSize / 2.0f, 0f, DrawMapHeight * tileSize / 2.0f);

        for (int x = 0; x < DrawMapWidth; x++)
        {
            for (int z = 0; z < DrawMapHeight; z++)
            {
                var pos = new Vector3(x * tileSize, 0, z * tileSize);
                MeshData meshData = map[z * DrawMapWidth + x];

                var tileObj = new GameObject($"({x}, 0, {z}) {meshData.name}");
                tileObj.layer = LayerMask.NameToLayer(groundLayer);
                tileObj.transform.position = pos - offset + meshData.translation;
                tileObj.transform.rotation = Quaternion.Euler(meshData.rotationAngles);
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = meshData.mesh;
                tileObj.AddComponent<MeshCollider>();
                tileObj.GetComponent<Renderer>().sharedMaterial = meshMaterial;

                if(meshData.name.Contains("Sea"))
                {
                    var nmm = tileObj.AddComponent<NavMeshModifier>();
                    nmm.overrideArea = true;
                    nmm.area = NavMesh.GetAreaFromName("Seaside");
                }

                // Seabed is created to a separate mesh, as only the ground should be navigable
                tileObj.transform.parent = meshData.name.StartsWith("Seabed") ? Seabed.transform : Ground.transform;
            }
            yield return null;
        }

        // Seabed can be merged to a single mesh
        MergeMeshes(Seabed);
        var seaNmm = Seabed.AddComponent<NavMeshModifier>();
        seaNmm.overrideArea = true;
        seaNmm.area = NavMesh.GetAreaFromName("Not Walkable");
        Seabed.layer = LayerMask.NameToLayer(seaLayer);
    }

    private void ClearMap()
    {
        while (Ground.transform.childCount > 0)
            DestroyImmediate(Ground.transform.GetChild(0).gameObject);

        while (Seabed.transform.childCount > 0)
            DestroyImmediate(Seabed.transform.GetChild(0).gameObject);
        
        DestroyImmediate(Ground.GetComponent<MeshFilter>());
        DestroyImmediate(Ground.GetComponent<MeshRenderer>());
        DestroyImmediate(Ground.GetComponent<MeshCollider>());
        DestroyImmediate(Ground.GetComponent<DebugMesh>());
        
        DestroyImmediate(Seabed.GetComponent<MeshFilter>());
        DestroyImmediate(Seabed.GetComponent<MeshRenderer>());
        DestroyImmediate(Seabed.GetComponent<MeshCollider>());
        DestroyImmediate(Seabed.GetComponent<DebugMesh>());
    }

    private void MergeMeshes(GameObject parent)
    {
        MeshFilter[] meshFilters = parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            DestroyImmediate(meshFilters[i].gameObject);

            i++;
        }

        var renderer = parent.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = meshMaterial;
        var meshFilter = parent.AddComponent<MeshFilter>();

        Mesh mesh = new ()
        {
            indexFormat = UnityEngine.Rendering.IndexFormat.UInt32
        };
        mesh.CombineMeshes(combine);
        meshFilter.sharedMesh = mesh;

        parent.AddComponent<MeshCollider>();

        if(addMeshDebugGizmos)
            parent.AddComponent<DebugMesh>();
    }

    public Rect GetSquareBounds(int gameX, int gameZ)
    {
        float meshX = (gameX * tileSize) - ((DrawMapWidth * tileSize) / 2.0f);
        float meshZ = (gameZ * tileSize) - ((DrawMapHeight * tileSize) / 2.0f);

        return new Rect(meshX, meshZ, tileSize, tileSize);
    }

    public Bounds GetIslandBounds()
    {
        var combinedBounds = new Bounds();

        foreach(var renderer in Ground.GetComponentsInChildren<Renderer>())
            combinedBounds.Encapsulate(renderer.bounds);

        return combinedBounds;
    }

    public float GetGridHeightAt(float worldX, float worldZ)
    { 
        Ray ray = new(new Vector3(worldX, 1000f, worldZ), Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hit, 2000f, LayerMask.GetMask(groundLayer, seaLayer))) 
        {
            return hit.point.y;
        }
        else
        {
            return float.NaN;
        }
    }

    public Vector2Int GetGameSquareFromWorldCoords(Vector3 point)
    {
        float x = point.x + (DrawMapWidth * tileSize / 2.0f);
        float y = point.z + (DrawMapHeight * tileSize / 2.0f);

        x /= tileSize;
        y /= tileSize;

        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }

    public IEnumerable<MeshRenderer> GetAllGroundMeshes()
    {
        return Ground.GetComponentsInChildren<MeshRenderer>();
    }
}
