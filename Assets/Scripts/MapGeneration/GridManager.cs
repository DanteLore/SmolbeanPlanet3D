using UnityEngine;
using System.Linq;
using System;
using Unity.AI.Navigation;

public class GridManager : MonoBehaviour
{
    public Material meshMaterial;
    public float fuzzyEdgeFactor = 0.01f;
    public float tileSize = 4.0f;

    [Range(0.0f, 1.0f)]
    public float coastRadius = 0.8f;

    public string groundLayer = "Ground";

    private System.Random rand = new System.Random();

    public bool addMeshDebugGizmos = false;

    private MeshData[] map;

    private GameObject Ground { get { return transform.Find("Ground").gameObject; }}
    private GameObject Seabed { get { return transform.Find("Seabed").gameObject; }}

    private GameMapGenerator gameMapGenerator;
    protected GameMapGenerator GameMapGenerator
    {
        get
        {
            if(!gameMapGenerator)
                gameMapGenerator = GetComponent<GameMapGenerator>();
            return gameMapGenerator;
        }
    }

    protected int GameMapWidth { get { return GameMapGenerator.mapWidth; }}
    protected int GameMapHeight { get { return GameMapGenerator.mapHeight; }}

    protected int DrawMapWidth { get { return GameMapWidth + 1; }}
    protected int DrawMapHeight { get { return GameMapHeight + 1; }}

    public void Recreate()
    {
        DateTime startTime = DateTime.Now;
        
        ClearMap();

        var meshData = GetComponent<MeshLoader>().LoadMeshes();
        print($"Loaded {meshData.Count()} meshes");

        var neighbourData = new NeighbourSelector(fuzzyEdgeFactor, meshData).SelectNeighbours();
        
        map = new MapGenerator(GameMapWidth, GameMapHeight, coastRadius, meshData, neighbourData).GenerateMap(GameMapGenerator.GameMap);

        DrawMap();

        UpdateNavMesh();

        Debug.Log($"Map generated in {(DateTime.Now - startTime).TotalSeconds}s");
    }

    private void UpdateNavMesh()
    {
        var surface = Ground.GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    private void DrawMap()
    {
        Vector3 offset = new Vector3((DrawMapWidth * tileSize) / 2.0f, 0.0f, (DrawMapHeight * tileSize) / 2.0f);

        for (int x = 0; x < DrawMapWidth; x++)
        {
            for (int z = 0; z < DrawMapHeight; z++)
            {
                // In future it might make sense to look at creating one big mesh here, rather than separate game objects then merging them... maybe.
                var pos = new Vector3(x * tileSize, 0, z * tileSize);
                MeshData meshData = map[z * DrawMapWidth + x];

                var tileObj = new GameObject($"({x}, 0, {z}) {meshData.name}");
                tileObj.layer = LayerMask.NameToLayer(groundLayer);
                tileObj.transform.position = pos - offset;
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = meshData.mesh;
                var collider = tileObj.AddComponent<MeshCollider>();
                tileObj.GetComponent<Renderer>().sharedMaterial = meshMaterial;

                // Seabed is created to a separate mesh, as only the ground should be navigable
                tileObj.transform.parent = meshData.name.StartsWith("Seabed") ? Seabed.transform : Ground.transform;
            }
        }

        MergeMeshes(Ground);
        MergeMeshes(Seabed);
    }

    private void ClearMap()
    {
        foreach(var gen in GetComponentsInChildren<IObjectGenerator>())
            gen.Clear();

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

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        meshFilter.sharedMesh = mesh;

        var collider = parent.AddComponent<MeshCollider>();

        if(addMeshDebugGizmos)
            parent.AddComponent<DebugMesh>();
    }

    internal Rect GetSquareBounds(int gameX, int gameZ)
    {
        float meshX = (gameX * tileSize) - ((DrawMapWidth * tileSize) / 2.0f);
        float meshZ = (gameZ * tileSize) - ((DrawMapHeight * tileSize) / 2.0f);

        return new Rect(meshX, meshZ, tileSize, tileSize);
    }

    internal float GetGridHeightAt(float worldX, float worldZ)
    { 
        Ray ray = new Ray(new Vector3(worldX, 100f, worldZ), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, LayerMask.GetMask(groundLayer))) 
        {
            return hit.point.y;
        }
        else
        {
            return float.NaN;
        }
    }

    internal Vector2Int GetGameSquareFromWorldCoords(Vector3 point)
    {
        float x = point.x + ((DrawMapWidth * tileSize) / 2.0f);
        float y = point.z + ((DrawMapHeight * tileSize) / 2.0f);

        x /= tileSize;
        y /= tileSize;

        return new Vector2Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }
}
