using UnityEngine;
using System.Linq;
using UnityEditor;
using System.Collections.Generic;
using System;

public class NeighbourSelector
{
    private float fuzzyEdgeFactor = 0.01f;
    private IEnumerable<MeshData> meshData;

    public NeighbourSelector(float fuzzyEdgeFactor, IEnumerable<MeshData> meshData)
    {
        this.fuzzyEdgeFactor = fuzzyEdgeFactor;
        this.meshData = meshData;
    }

    public IEnumerable<NeighbourData> SelectNeighbours()
    {
        return meshData.Select(CreateNeighbourData);
    }

    private NeighbourData CreateNeighbourData(MeshData target)
    {
        var cornerLevelData = GetCornerLevelData(target);

        return new NeighbourData
        {
            id = target.name.GetHashCode(),
            name = target.name,
            leftMatches = meshData.Where(md => BoundariesMatch(target.leftBoundary, md.rightBoundary)).Select(meshData => meshData.id).ToList(),
            rightMatches = meshData.Where(md => BoundariesMatch(target.rightBoundary, md.leftBoundary)).Select(meshData => meshData.id).ToList(),
            frontMatches = meshData.Where(md => BoundariesMatch(target.frontBoundary, md.backBoundary)).Select(meshData => meshData.id).ToList(),
            backMatches = meshData.Where(md => BoundariesMatch(target.backBoundary, md.frontBoundary)).Select(meshData => meshData.id).ToList(),
            backLeftLevel = cornerLevelData[0],
            backRightLevel = cornerLevelData[1],
            frontleftLevel = cornerLevelData[2],
            frontRightLevel = cornerLevelData[3]
        };
    }

    private int[] GetCornerLevelData(MeshData target)
    {
        float meshHeight = 2.0f;

        return new int[] {
            Mathf.FloorToInt((target.backLeftHeight) / meshHeight) + 1,
            Mathf.FloorToInt((target.backRightHeight) / meshHeight) + 1,
            Mathf.FloorToInt((target.frontLeftHeight) / meshHeight) + 1,
            Mathf.FloorToInt((target.frontRightHeight) / meshHeight) + 1
        };
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
}
