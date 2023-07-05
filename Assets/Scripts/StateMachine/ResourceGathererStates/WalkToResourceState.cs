using UnityEngine;
using UnityEngine.AI;

public class WalkToResourceState : WalkStateBase
{
    private ResourceGatherer gatherer;

    public WalkToResourceState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(navAgent, animator, soundPlayer)
    {
        this.gatherer = gatherer;
    }

    protected override Vector3 GetDestination()
    {
        return gatherer.Target.transform.position;
    }
}