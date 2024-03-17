using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FindRestingPlaceState : IState
{
    private readonly SmolbeanColonist colonist;

    public FindRestingPlaceState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    { 
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        FindPlaceToGo();
    }

    // TODO: Come back to this when there is somewhere for colonists to rest!
    private void FindPlaceToGo()
    {
        var pos = ShipwreckManager.Instance.Shipwreck.spawnPoint.transform.position;

        float radius = 12f;
        float x = pos.x + Random.Range(-radius, radius);
        float z = pos.z + Random.Range(-radius, radius);

        if (
                Physics.Raycast(new Ray(new Vector3(x, 1000, z), Vector3.down), out var rayHit, 2000, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(rayHit.point, out var navHit, 1.0f, NavMesh.AllAreas)
                && navHit.position.y > 0.0f //don't go into the sea!
            )
            colonist.target = rayHit.point;
    }
}
