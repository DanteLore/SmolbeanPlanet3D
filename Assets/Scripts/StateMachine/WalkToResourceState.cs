using UnityEngine;
using UnityEngine.AI;

public class WalkToResourceState : WalkStateBase
{
    public WalkToResourceState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(gatherer, navAgent, animator, soundPlayer) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.Target.transform.position;
    }
}