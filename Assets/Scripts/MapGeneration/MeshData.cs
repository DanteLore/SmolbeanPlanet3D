using UnityEngine;

public class MeshData
{
    public string name;
    public Mesh mesh;
    public Edge[] edges;
    public Edge[] leftBoundary;
    public Edge[] rightBoundary;
    public Edge[] frontBoundary;
    public Edge[] backBoundary;
    public float backLeftHeight;
    public float backRightHeight;
    public float frontLeftHeight;
    public float frontRightHeight;
}
