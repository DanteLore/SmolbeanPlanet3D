using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Unity.VisualScripting;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMeshGenerator : MonoBehaviour
{
    [Min(4)]
    public int numVertices = 4;

    public float widthHeight = 100.0f;

    public bool makeCircularMesh = false;

    public float bowlHeight = 0;

    public void GeneratePlane()
    {
        // Create new vertices
        Vector3[] vertices = new Vector3[numVertices * numVertices];
        Vector2[] uv = new Vector2[numVertices * numVertices];
        Vector3[] normals = new Vector3[numVertices * numVertices];

        float unitSize = widthHeight / (numVertices - 1);
        float radius = widthHeight / 2;
        float offset = radius * -1;
        Vector3 meshCenter = Vector3.zero;
        Vector2 flatMeshCenter = new Vector2(meshCenter.x, meshCenter.z);

        for (int z = 0; z < numVertices; z++)
        {
            for (int x = 0; x < numVertices; x++)
            {
                var point = new Vector3(offset + x * unitSize, 0f, offset + z * unitSize);
                float d = Vector3.Distance(point, meshCenter) / radius;
                float p = Mathf.Pow(d, 3);
                point.y -= p * bowlHeight;
                
                vertices[z * numVertices + x] = point;
                uv[z * numVertices + x] = new Vector2(x / widthHeight, z / widthHeight);
            }
        }

        // Create new triangles
        var triangles = new List<int>();

        for (int z = 0; z < numVertices - 1; z++)
        {
            for (int x = 0; x < numVertices - 1; x++)
            {
                int bottomLeftIndex = z * numVertices + x;
                int bottomRightIndex = z * numVertices + x + 1;
                int topLeftIndex = (z + 1) * numVertices + x;
                int topRightIndex = (z + 1) * numVertices + x + 1;

                var center = (vertices[bottomLeftIndex] + vertices[bottomLeftIndex] + vertices[topLeftIndex] + vertices[bottomRightIndex]) / 4f;

                if(!makeCircularMesh || Vector2.Distance(new Vector2(center.x, center.z), flatMeshCenter) < radius)
                {
                    triangles.Add(bottomLeftIndex);
                    triangles.Add(topLeftIndex);
                    triangles.Add(topRightIndex);

                    triangles.Add(bottomLeftIndex);
                    triangles.Add(topRightIndex);
                    triangles.Add(bottomRightIndex);
                }
            }
        }

        // Setup normals
        for (int i = 0; i < triangles.Count; i += 3)
        {
            int va = triangles[i];
            int vb = triangles[i + 1];
            int vc = triangles[i + 2];

            Vector3 pointA = vertices[va];
            Vector3 pointB = vertices[vb];
            Vector3 pointC = vertices[vc];

            Vector3 sideAB = pointB - pointA;
            Vector3 sideAC = pointC - pointA;

            var triangleNormal = Vector3.Cross(sideAB, sideAC).normalized;

            normals[va] += triangleNormal;
            normals[vb] += triangleNormal;
            normals[vc] += triangleNormal;
        }

        for(int i = 0; i < normals.Length; i++)
            normals[i].Normalize();

        // Create the mesh
        var mesh = new Mesh
        {
            vertices = vertices,
            triangles = triangles.ToArray(),
            normals = normals,
            uv = uv
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }
}