using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class MeshData
{
    public int id;
    public string name;
    public Mesh mesh;
    [NonSerialized] public Edge[] edges;
    [NonSerialized] public Edge[] leftBoundary;
    [NonSerialized] public Edge[] rightBoundary;
    [NonSerialized] public Edge[] frontBoundary;
    [NonSerialized] public Edge[] backBoundary;
    public float backLeftHeight;
    public float backRightHeight;
    public float frontLeftHeight;
    public float frontRightHeight;
    public Vector3 translation;
    public Vector3 rotationAngles;
}

