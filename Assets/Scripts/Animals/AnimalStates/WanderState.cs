using UnityEngine;
using UnityEngine.AI;

public class WanderState : WalkStateBase
{
    private readonly SmolbeanAnimal animal;

    public WanderState(SmolbeanAnimal animal, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
        : base(navAgent, animator, soundPlayer)
    {
        this.animal = animal;
    }

    public override void OnEnter()
    {
        base.OnEnter();

        var pos = animal.transform.position;

        float x = pos.x + Random.Range(-4f, 4f);
        float z = pos.z + Random.Range(-4f, 4f);

        if (Physics.Raycast(new Ray(new Vector3(x, 1000, z), Vector3.down), out var rayHit, 2000, LayerMask.GetMask("Ground"))
            && NavMesh.SamplePosition(rayHit.point, out var navHit, 1.0f, NavMesh.AllAreas))
            animal.target = navHit.position;
        else
            animal.target = pos;
    }
    
    protected override Vector3 GetDestination()
    {
        return animal.target;
    }
}
