using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class FarmerSelectFieldCenterState : IState
{
    internal struct SearchResult
    {
        internal Vector3 pos;
        internal float grassQtty;
    }

    private readonly float fieldRadius;
    private readonly Farmer farmer;
    private int searchAttemptCount;
    private readonly List<SearchResult> fieldLocations = new();

    public bool FieldFound { get; private set; }
    public bool InProgress { get; private set; }

    public FarmerSelectFieldCenterState(Farmer farmer, float fieldRadius)
    {
        this.farmer = farmer;
        this.fieldRadius = fieldRadius;
    }

    public void OnEnter()
    {
        farmer.target = farmer.transform.position;
        FieldFound = false;
        searchAttemptCount = 0;
        fieldLocations.Clear();
        InProgress = true;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        if (searchAttemptCount < 60)
        {
            FindFieldLocation();
            return;
        }

        if (fieldLocations.Any())
        {
            FieldFound = true;
            var choices = fieldLocations
                .OrderByDescending(fl => fl.grassQtty)
                .Take(5)
                .ToArray();

            farmer.target = choices[Random.Range(0, choices.Length)].pos;
            farmer.fieldCenter = farmer.target;
        }
        else
        {
            FieldFound = false;
            farmer.target = farmer.transform.position;
        }

        InProgress = false;
    }

    private void FindFieldLocation()
    {
        searchAttemptCount++;

        var barn = (Barn)farmer.Job.Building;

        Vector3 center = barn.collectionZoneCenter;
        float range = Random.Range(0f, barn.collectionZoneRadius);
        float angle = Random.Range(0f, 360f);

        var pos = center + (Quaternion.AngleAxis(angle, Vector3.up) * (Vector3.forward * range));

        Ray ray = new(pos + (Vector3.up * 1000f), Vector3.down);
        if (Physics.Raycast(ray, out var rayHit, float.MaxValue, LayerMask.GetMask(farmer.groundLayer)) &&
            Physics.OverlapSphere(rayHit.point, fieldRadius, LayerMask.GetMask(farmer.buildingLayer, farmer.natureLayer)).Length == 0 &&
            NavMesh.SamplePosition(rayHit.point, out var navHit, 1f, NavMesh.AllAreas) &&
            navHit.position.y >= -0.5f // Not in the sea :)
            )
        {
            float grassQtty = GroundWearManager.Instance.GetAvailableGrass(navHit.position, fieldRadius);

            if(grassQtty >= 50f)
                fieldLocations.Add(new SearchResult { pos = navHit.position, grassQtty = grassQtty });
        }
    }
}
