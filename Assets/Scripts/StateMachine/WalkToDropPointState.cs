using UnityEngine;
using UnityEngine.AI;

public class WalkToDropPointState : WalkStateBase
{
    public WalkToDropPointState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator) 
        : base(gatherer, navAgent, animator) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.DropPoint;
    }
}
