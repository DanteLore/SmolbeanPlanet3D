using UnityEngine;
using UnityEngine.AI;

public class WalkToResourceState : WalkStateBase
{
    public WalkToResourceState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator) 
        : base(gatherer, navAgent, animator) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.Target.transform.position;
    }
}