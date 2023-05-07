using UnityEngine;
using System.Linq;
using System;

public class GridManager : MonoBehaviour
{
    public Material meshMaterial;
    public float tileSize = 4.0f;
    public float fuzzyEdgeFactor = 0.01f;

    [Range(0.0f, 1.0f)]
    public float coastRadius = 0.8f;

    public bool mergeMeshes = true;

    public string groundLayer = "Ground";

    private System.Random rand = new System.Random();

    public bool addMeshDebugGizmos = false;

    private MeshData[] map;

    private GameObject ground;
    private GameObject Ground 
    {
        get
        {
            if(ground == null)
            {
                var tf = transform.Find("Ground");

                if(tf != null)
                {
                    ground = tf.gameObject;
                }
                else
                {
                    ground = new GameObject();
                    ground.name = "Ground";
                    ground.layer = LayerMask.NameToLayer(groundLayer);
                    ground.transform.parent = transform;
                }
            }

            return ground;
        }
    }

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
        ClearMap();

        var meshData = new MeshLoader(fuzzyEdgeFactor).LoadMeshes();
        print($"Loaded {meshData.Count()} meshes");

        var neighbourData = new NeighbourSelector(fuzzyEdgeFactor, meshData).SelectNeighbours();

        //var nd = neighbourData["SeaSlopeToCliffTransition"];
        //Debug.Log("Left: " + String.Join(", ", nd.leftMatches));
        //Debug.Log("Right: " + String.Join(", ", nd.rightMatches));
        //Debug.Log("Front: " + String.Join(", ", nd.frontMatches));
        //Debug.Log("Back: " + String.Join(", ", nd.backMatches));

        map = new MapGenerator(GameMapWidth, GameMapHeight, coastRadius, meshData, neighbourData).GenerateMap(GameMapGenerator.GameMap);
        DrawMap();
    }

    private void DrawMap()
    {
        Vector3 offset = new Vector3((DrawMapWidth * tileSize) / 2.0f, 0.0f, (DrawMapHeight * tileSize) / 2.0f);

        for (int x = 0; x < DrawMapWidth; x++)
        {
            for (int z = 0; z < DrawMapHeight; z++)
            {
                // In future it might make sense to look at creating one big mesh here, rather than separate game objects... maybe.
                var pos = new Vector3(x * tileSize, 0, z * tileSize);
                MeshData meshData = map[z * DrawMapWidth + x];

                var tileObj = new GameObject();
                tileObj.layer = LayerMask.NameToLayer(groundLayer);
                tileObj.transform.position = pos - offset;
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = meshData.mesh;
                var collider = tileObj.AddComponent<MeshCollider>();
                tileObj.GetComponent<Renderer>().sharedMaterial = meshMaterial;

                if(addMeshDebugGizmos)
                    tileObj.AddComponent<DebugMesh>();

                tileObj.transform.parent = Ground.transform;
                tileObj.name = $"({x}, 0, {z}) {meshData.name}";
            }
        }

        if(mergeMeshes)
            MergeMeshes();
    }

    private void ClearMap()
    {
        foreach(var gen in GetComponentsInChildren<IObjectGenerator>())
            gen.Clear();

        while (Ground.transform.childCount > 0)
            DestroyImmediate(Ground.transform.GetChild(0).gameObject);
        
        DestroyImmediate(Ground);
    }

    private void MergeMeshes()
    {
        MeshFilter[] meshFilters = Ground.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            DestroyImmediate(meshFilters[i].gameObject);

            i++;
        }

        var renderer = Ground.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = meshMaterial;
        var meshFilter = Ground.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(combine);
        meshFilter.sharedMesh = mesh;

        var collider = Ground.AddComponent<MeshCollider>();
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
}
