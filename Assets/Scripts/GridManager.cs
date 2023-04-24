using UnityEngine;
using System.Linq;
using UnityEditor;
using System;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public struct Edge
    {
        public Vector3 start;
        public Vector3 end;

        public override string ToString()
        {
            return $"Edge {start.x},{start.y},{start.z} --> {end.x},{end.y},{end.z}";
        }
    }

    public struct MeshData
    {
        public string name;
        public Mesh mesh;
        public Edge[] edges;
        public Edge[] leftBoundary;
        public Edge[] rightBoundary;
        public Edge[] frontBoundary;
        public Edge[] backBoundary;
    }

    public Material meshMaterial;

    public int mapWidth = 10;
    public int mapHeight = 10;
    public float tileSize = 4.0f;
    public float fuzzyEdgeFactor = 0.01f;

    private List<MeshData> meshData;
    private System.Random rand = new System.Random();

    public void Recreate()
    {
        LoadMeshes();
        ClearMap();
        DrawMap();
    }

    private void LoadMeshes()
    {
        // Loading this from Resources rather than using an editor property seems a bit nasty, but it works, and I can't find anything else that does.
        var meshesInFile = Resources.LoadAll<Mesh>("TileMeshes"); // Passed here is the name of the folder that contains the FBX file. within '/Resources'.

        // Make copies of the meshes in the file, so any transformations etc don't mess things up
        var meshes = meshesInFile.Select(CloneMesh).ToList();

        // Create rotated versions of the meshes
        meshes = meshes.SelectMany(MeshRotatedFourWays).ToList();

        // This is the one we're using, for now.
        meshData = meshes.Select(CreateMeshData).ToList();

        print($"Loaded {meshes.Count()} meshes");

        // So it appears that X and Z axes are flipped when loading blender assets to unity.

        var target = meshData.First(md => md.name == "BumpyFloor");

        print("LEFT: " + String.Join(", ", target.leftBoundary));
        print("RIGHT: " + String.Join(", ", target.rightBoundary));
        print("FRONT: " + String.Join(", ", target.frontBoundary));
        print("BACK: " + String.Join(", ", target.backBoundary));

        var leftMatches = meshData.Where(md => BoundariesMatch(target.leftBoundary, md.rightBoundary)).Select(meshData => meshData.name);
        var rightMatches = meshData.Where(md => BoundariesMatch(target.rightBoundary, md.leftBoundary)).Select(meshData => meshData.name);
        var frontMatches = meshData.Where(md => BoundariesMatch(target.frontBoundary, md.backBoundary)).Select(meshData => meshData.name);
        var backMatches = meshData.Where(md => BoundariesMatch(target.backBoundary, md.frontBoundary)).Select(meshData => meshData.name);

        print("LEFT MATCHES: " + String.Join(", ", leftMatches));
        print("RIGHT MATCHES: " + String.Join(", ", rightMatches));
        print("FRONT MATCHES: " + String.Join(", ", frontMatches));
        print("BACK MATCHES: " + String.Join(", ", backMatches));

        // TODO:  Create rotated versions of all tiles
        // TODO:  Add the allowed neighbours to the MeshData
        // TODO:  Think about how to solve for multiple Y steps (maybe just clone and raise the tiles n times??)
        // TODO:  Seed the outside edge of the map with flat squares (in future seed with ocean, but for now, grass will do!)
        // TODO:  Wave function collapse from there, using the meshData!
    }

    private IEnumerable<Mesh> MeshRotatedFourWays(Mesh mesh)
    {
        yield return mesh;

        yield return RotateMesh(CloneMesh(mesh), 90);
        yield return RotateMesh(CloneMesh(mesh), 180);
        yield return RotateMesh(CloneMesh(mesh), 270);
    }

    private Mesh RotateMesh(Mesh mesh, float angleDegrees)
    {
        mesh.name = mesh.name + $" rotated {angleDegrees}";

        var rot = Quaternion.Euler(0.0f, angleDegrees, 0.0f);
        var rotated = mesh.vertices.Select(v => rot * v).ToArray();
        mesh.vertices = rotated;

        return mesh;
    }

    private bool BoundariesMatch(Edge[] boundary1, Edge[] boundary2)
    {
        var shorter = (boundary1.Length < boundary2.Length) ? boundary1 : boundary2;
        var longer = (boundary1.Length < boundary2.Length) ? boundary2 : boundary1;

        // Create a list of all the vertices along the edge with more points
        var longPoints = longer.Select(e => e.start).ToList();
        longPoints.Add(longer.Last().end);

        var error = longPoints.Sum(p => DistanceFromPointToBoundary(p, shorter));
        error = error / longPoints.Count();

        return error <= fuzzyEdgeFactor;
    }

    private float DistanceFromPointToBoundary(Vector3 point, Edge[] boundary)
    {
        return boundary.Min(e => DistanceFromPointToEdge(point, e));
    }

    private float DistanceFromPointToEdge(Vector3 point, Edge edge)
    {
        var line = (edge.end - edge.start);
        var len = line.magnitude;
        line.Normalize();
    
        var v = point - edge.start;
        var d = Vector3.Dot(v, line);
        d = Mathf.Clamp(d, 0f, len);
        var intersect = edge.start + line * d;

        return Vector3.Distance(point, intersect);
    }

    private Mesh CloneMesh(Mesh m)
    {
        return new Mesh()
        {
            name = m.name,
            vertices = m.vertices,
            triangles = m.triangles,
            normals = m.normals,
            tangents = m.tangents,
            bounds = m.bounds,
            uv = m.uv
        };
    }

    private MeshData CreateMeshData(Mesh mesh)
    {
        var edges = new List<Edge>();
        for(int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Vector3 p1 = mesh.vertices[mesh.triangles[i]];
            Vector3 p2 = mesh.vertices[mesh.triangles[i + 1]];
            Vector3 p3 = mesh.vertices[mesh.triangles[i + 2]];

            edges.Add(new Edge { start = p1, end = p2 });
            edges.Add(new Edge { start = p2, end = p3 });
            edges.Add(new Edge { start = p3, end = p1 });
        }

        var leftBoundary = edges
            .Where(e => Mathf.Abs(e.start.x - mesh.bounds.min.x) <= fuzzyEdgeFactor && Mathf.Abs(e.end.x - mesh.bounds.min.x) <= fuzzyEdgeFactor)
            .Select(e => new Edge {start = new Vector3(0.0f, e.start.y, e.start.z), end = new Vector3(0.0f, e.end.y, e.end.z)});
        var rightBoundary = edges
            .Where(e => Mathf.Abs(e.start.x - mesh.bounds.max.x) <= fuzzyEdgeFactor && Mathf.Abs(e.end.x - mesh.bounds.max.x) <= fuzzyEdgeFactor)
            .Select(e => new Edge {start = new Vector3(0.0f, e.start.y, e.start.z), end = new Vector3(0.0f, e.end.y, e.end.z)});
        var frontBoundary = edges
            .Where(e => Mathf.Abs(e.start.z - mesh.bounds.min.z) <= fuzzyEdgeFactor && Mathf.Abs(e.end.z - mesh.bounds.min.z) <= fuzzyEdgeFactor)
            .Select(e => new Edge {start = new Vector3(e.start.x, e.start.y, 0.0f), end = new Vector3(e.end.x, e.end.y, 0.0f)});
        var backBoundary = edges
            .Where(e => Mathf.Abs(e.start.z - mesh.bounds.max.z) <= fuzzyEdgeFactor && Mathf.Abs(e.end.z - mesh.bounds.max.z) <= fuzzyEdgeFactor)
            .Select(e => new Edge {start = new Vector3(e.start.x, e.start.y, 0.0f), end = new Vector3(e.end.x, e.end.y, 0.0f)});

        return new MeshData
            {
                name = mesh.name,
                mesh = mesh,
                edges = edges.ToArray(),
                leftBoundary = leftBoundary.ToArray(),
                rightBoundary = rightBoundary.ToArray(),
                frontBoundary = frontBoundary.ToArray(),
                backBoundary = backBoundary.ToArray()
            };
    }

    private void DrawMap()
    {
        Vector3 offset = new Vector3((mapWidth * tileSize) / 2, 0.0f, (mapHeight * tileSize) / 2);

        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                // In future it might make sense to look at creating one big mesh here, rather than separate game objects... maybe.
                var pos = new Vector3(x * tileSize, 0, z * tileSize);
                Mesh mesh = GetMeshFor(x, z);

                var tileObj = new GameObject();
                tileObj.transform.position = pos - offset;
                tileObj.AddComponent<MeshRenderer>();
                var meshFilter = tileObj.AddComponent<MeshFilter>();
                meshFilter.mesh = mesh;
                var collider = tileObj.AddComponent<MeshCollider>();
                //collider.sharedMesh = floorMesh;
                tileObj.GetComponent<Renderer>().material = meshMaterial;

                tileObj.transform.parent = transform;
                tileObj.name = $"Terrain cube {x}, 0, {z}";
            }
        }
    }

    private Mesh GetMeshFor(int x, int z)
    {
        return meshData[rand.Next(meshData.Count)].mesh;
    }

    private void ClearMap()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }
}

