using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;

public class MeshLoader
{
    private float fuzzyEdgeFactor;

    public MeshLoader(float fuzzyEdgeFactor)
    {
        this.fuzzyEdgeFactor = fuzzyEdgeFactor;
    }

    public List<MeshData> LoadMeshes()
    {
        // Loading this from Resources rather than using an editor property seems a bit nasty, but it works, and I can't find anything else that does.
        var meshesInFile = Resources.LoadAll<Mesh>("TileMeshes"); // Passed here is the name of the folder that contains the FBX file. within '/Resources'.

        // Make copies of the meshes in the file, so any transformations etc don't mess things up
        var meshes = meshesInFile.Select(CloneMesh).ToList();

        // Create rotated versions of the meshes
        meshes = meshes.SelectMany(MeshRotatedFourWays).ToList();

        // This is the one we're using, for now.
        var meshData = meshes.Select(CreateMeshData).ToList();

        return meshData;
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
}

