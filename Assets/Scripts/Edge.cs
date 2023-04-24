using UnityEngine;

public struct Edge
{
    public Vector3 start;
    public Vector3 end;

    public override string ToString()
    {
        return $"Edge {start.x},{start.y},{start.z} --> {end.x},{end.y},{end.z}";
    }
}

