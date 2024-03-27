using UnityEngine;
using UnityEngine.AI;

public class FindNextSpotInFieldState : IState
{
    private readonly Farmer farmer;

    public FindNextSpotInFieldState(Farmer farmer)
    {
        this.farmer = farmer;
    }

    public bool LocationFound { get; private set; }

    public void OnEnter()
    {
        LocationFound = false;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        float t = Vector3.Distance(farmer.target, farmer.fieldCenter / farmer.fieldRadius);
        float angle = Mathf.Lerp(360f / (2f * Mathf.PI * farmer.fieldRadius), 45f, t);
        
        Quaternion turn = Quaternion.AngleAxis(angle, Vector3.up);
        var pos = farmer.transform.position + (farmer.transform.rotation * turn * Vector3.forward);

        if(NavMesh.SamplePosition(pos, out var navHit, 1f, NavMesh.AllAreas))
        {
            LocationFound = true;
            farmer.target = navHit.position;
        }
        else
        {
            LocationFound = false;
            farmer.target = farmer.transform.position;
        }
    }
}
