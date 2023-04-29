using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMeshGenerator : MonoBehaviour
{
    public int numVertices = 4;

    public float widthHeight = 100.0f;

    public void GeneratePlane()
    {
        var mesh = new Mesh();
        // Create new vertices
        Vector3[] vertices = new Vector3[numVertices * numVertices];
        Vector2[] uv = new Vector2[numVertices * numVertices];
        float unitSize = widthHeight / (numVertices - 1);
        
        for (int y = 0; y < numVertices; y++)
        {
            for (int x = 0; x < numVertices; x++)
            {
                vertices[y * numVertices + x] = new Vector3(x * unitSize, 0f, y * unitSize);
                uv[y * numVertices + x] = new Vector2(x / widthHeight, y / widthHeight);
            }
        }

        // Create new triangles
        int[] triangles = new int[(numVertices - 1) * (numVertices - 1) * 6];

        int triangleIndex = 0;
        for (int y = 0; y < numVertices - 1; y++)
        {
            for (int x = 0; x < numVertices - 1; x++)
            {
                int bottomLeftIndex = y * numVertices + x;
                int bottomRightIndex = y * numVertices + x + 1;
                int topLeftIndex = (y + 1) * numVertices + x;
                int topRightIndex = (y + 1) * numVertices + x + 1;

                triangles[triangleIndex++] = bottomLeftIndex;
                triangles[triangleIndex++] = topLeftIndex;
                triangles[triangleIndex++] = topRightIndex;

                triangles[triangleIndex++] = bottomLeftIndex;
                triangles[triangleIndex++] = topRightIndex;
                triangles[triangleIndex++] = bottomRightIndex;
            }
        }

        Vector3[] normals = new Vector3[numVertices * numVertices];
        for (int i = 0; i < numVertices * numVertices; i++)
        {
            normals[i] = Vector3.up;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        GetComponent<MeshFilter>().mesh = mesh;
    }
}