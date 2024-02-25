using UnityEngine;
using UnityEngine.AI;

public class ChooseEatingPlaceState : IState
{
    private readonly SmolbeanAnimal animal;

    public ChooseEatingPlaceState(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        var pos = animal.transform.position;

        float x = pos.x + Random.Range(-4f, 4f);
        float z = pos.z + Random.Range(-4f, 4f);

        if (
                Physics.Raycast(new Ray(new Vector3(x, 1000, z), Vector3.down), out var rayHit, 2000, LayerMask.GetMask("Ground"))
                && NavMesh.SamplePosition(rayHit.point, out var navHit, 1.0f, NavMesh.AllAreas)
                && navHit.position.y > 0.0f //don't go into the sea!
            )
            animal.target = navHit.position;
        else
            animal.target = pos;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
