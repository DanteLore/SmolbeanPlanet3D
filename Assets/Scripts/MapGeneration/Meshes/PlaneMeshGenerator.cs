using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMeshGenerator : MonoBehaviour
{
    [Min(4)]
    public int numVertices = 4;

    public float widthHeight = 100.0f;

    public bool makeCircularMesh = false;

    public void GeneratePlane()
    {
        // Create new vertices
        Vector3[] vertices = new Vector3[numVertices * numVertices];
        Vector2[] uv = new Vector2[numVertices * numVertices];
        float unitSize = widthHeight / (numVertices - 1);
        float radius = widthHeight / 2;
        float offset = radius * -1;
        Vector3 meshCenter = Vector3.zero;
        
        for (int y = 0; y < numVertices; y++)
        {
            for (int x = 0; x < numVertices; x++)
            {
                vertices[y * numVertices + x] = new Vector3(offset + x * unitSize, 0f, offset + y * unitSize);
                uv[y * numVertices + x] = new Vector2(x / widthHeight, y / widthHeight);
            }
        }

        // Create new triangles
        var triangles = new List<int>((numVertices - 1) * (numVertices - 1) * 6);

        for (int y = 0; y < numVertices - 1; y++)
        {
            for (int x = 0; x < numVertices - 1; x++)
            {
                int bottomLeftIndex = y * numVertices + x;
                int bottomRightIndex = y * numVertices + x + 1;
                int topLeftIndex = (y + 1) * numVertices + x;
                int topRightIndex = (y + 1) * numVertices + x + 1;

                var center = (vertices[bottomLeftIndex] + vertices[bottomLeftIndex] + vertices[topLeftIndex] + vertices[bottomRightIndex]) / 4f;

                if(!makeCircularMesh || Vector3.Distance(center, meshCenter) < radius)
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
        Vector3[] normals = new Vector3[numVertices * numVertices];
        for (int i = 0; i < numVertices * numVertices; i++)
        {
            normals[i] = Vector3.up;
        }

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