using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class FindNextSpotInFieldState : IState
{
    private readonly Farmer farmer;
    private readonly float swathRadius;
    private List<Vector3> waypoints;
    private int currentIndex;

    public string Name { get => GetType().Name; }
    public bool LocationFound { get; private set; }

    public FindNextSpotInFieldState(Farmer farmer)
    {
        this.farmer = farmer;
    }

    public void OnEnter()
    {
        LocationFound = false;

        waypoints = GenerateCircularPath(farmer.fieldCenter, farmer.fieldRadius, farmer.swathRadius);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (currentIndex >= waypoints.Count)
            return; // We're done!

        Vector3 target = waypoints[currentIndex];
        Vector3 pos = farmer.transform.position;
        Vector3 nextStep = (target - pos).normalized * swathRadius + pos;
        if (NavMesh.SamplePosition(nextStep, out var hit, 1f, NavMesh.AllAreas))
        {
            LocationFound = true;
            farmer.Target = hit.position;
        }
        else
        {
            LocationFound = false;
            farmer.Target = farmer.transform.position;
        }
    }
    
    List<Vector3> GenerateCircularPath(Vector3 center, float radius, float swathWidth)
    {
        var pts = new List<Vector3>();
        
        int rows = Mathf.CeilToInt( 2f * radius / swathWidth );
        for(int i=0; i<=rows; i++)
        {
            float z = -radius + i * swathWidth;
            // half‑width at this z
            float halfX = Mathf.Sqrt(radius * radius - z * z);

            // build the two endpoints of this “strip”
            Vector3 left  = center + new Vector3(-halfX, 0, z);
            Vector3 right = center + new Vector3( halfX, 0, z);

            // zig‑zag: even rows go L→R, odd rows go R→L
            if (i % 2 == 0)
            {
                pts.Add(left);
                pts.Add(right);
            }
            else
            {
                pts.Add(right);
                pts.Add(left);
            }
        }
        return pts;
    }
}
