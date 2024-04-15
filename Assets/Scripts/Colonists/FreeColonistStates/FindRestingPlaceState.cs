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
        // TODO:  This would probably go better in the idle state, but these deep thoughts might be too much for a Dodo!
        if(Random.Range(0f, 10f) >= 5f)
            colonist.Think(RandomThoughtGenerator.GetThought());
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
        var pos = colonist.Home.GetComponent<SmolbeanBuilding>().GetSpawnPoint();

        float radius = 12f;
        float x = pos.x + Random.Range(-radius, radius);
        float z = pos.z + Random.Range(-radius, radius);

        if (
                Physics.Raycast(new Ray(new Vector3(x, 5000f, z), Vector3.down), out var rayHit, float.MaxValue, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(rayHit.point, out var navHit, 1.0f, NavMesh.AllAreas)
                && navHit.position.y > 0.0f //don't go into the sea!
            )
            colonist.target = rayHit.point;
    }
}
