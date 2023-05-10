using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

public class MeshLoader : MonoBehaviour
{
    public float fuzzyEdgeFactor = 0.01f;
    public float flatnessThreshold = 0.5f;

    public string assetPath = "Assets/Meshes/CubicTerrain.fbx";

    public List<MeshData> LoadMeshes()
    {

        // Still more work to do here. Really, this needs to be an editor script which loads the meshes into the GridManager for use either at runtime or in edit mode.

#if UNITY_EDITOR
            var meshesInFile = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                .Where(o => o is Mesh)
                .Select(m => (Mesh)m);
#else
            var meshesInFile = new Mesh[0];
#endif

       return meshesInFile
            .Select(CloneMesh)
            .SelectMany(MeshRotatedFourWays)
            .Select(CreateMeshData).ToList();
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

    private IEnumerable<Mesh> MeshRotatedFourWays(Mesh mesh)
    {
        yield return mesh;

        bool isRoughlyFlat = (mesh.vertices.Max(v => v.y) - mesh.vertices.Min(v => v.y)) <= flatnessThreshold;
        if(isRoughlyFlat) // No need to create rotated versions if this tile is basically flat
            yield break;

        yield return RotateMesh(CloneMesh(mesh), 90);
        yield return RotateMesh(CloneMesh(mesh), 180);
        yield return RotateMesh(CloneMesh(mesh), 270);
    }

    private Mesh RotateMesh(Mesh mesh, float angleDegrees)
    {
        mesh.name = mesh.name + $" rotated {angleDegrees}";

        var rot = Quaternion.Euler(0.0f, angleDegrees, 0.0f);
        var rotatedVerts = mesh.vertices.Select(v => rot * v).ToArray();
        mesh.vertices = rotatedVerts;
        var rotatedNormals = mesh.normals.Select(v => rot * v).ToArray();
        mesh.normals = rotatedNormals;

        return mesh;
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

        float maxX = mesh.vertices.Max(v => v.x);
        float maxZ = mesh.vertices.Max(v => v.z);
        float minX = mesh.vertices.Min(v => v.x);
        float minZ = mesh.vertices.Min(v => v.z);

        Vector3 backLeft = new Vector3(minX, 0.0f, maxZ);
        Vector3 backRight = new Vector3(maxX, 0.0f, maxZ);
        Vector3 frontLeft = new Vector3(minX, 0.0f, minZ);
        Vector3 frontRight = new Vector3(maxX, 0.0f, minZ);

        var backLeftVertex = mesh.vertices.OrderBy(v => (new Vector3(v.x, 0.0f, v.z) - backLeft).sqrMagnitude).First();
        var backRightVertex = mesh.vertices.OrderBy(v => (new Vector3(v.x, 0.0f, v.z) - backRight).sqrMagnitude).First();
        var frontLeftVertex = mesh.vertices.OrderBy(v => (new Vector3(v.x, 0.0f, v.z) - frontLeft).sqrMagnitude).First();
        var frontRightVertex = mesh.vertices.OrderBy(v => (new Vector3(v.x, 0.0f, v.z) - frontRight).sqrMagnitude).First();

        return new MeshData
            {
                id = mesh.name.GetHashCode(),
                name = mesh.name,
                mesh = mesh,
                edges = edges.ToArray(),
                leftBoundary = leftBoundary.ToArray(),
                rightBoundary = rightBoundary.ToArray(),
                frontBoundary = frontBoundary.ToArray(),
                backBoundary = backBoundary.ToArray(),
                backLeftHeight = backLeftVertex.y,
                backRightHeight = backRightVertex.y,
                frontLeftHeight = frontLeftVertex.y,
                frontRightHeight = frontRightVertex.y
            };
    }
}

