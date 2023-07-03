using UnityEngine;
using UnityEngine.AI;

public class WalkHomeState : WalkStateBase
{
    public WalkHomeState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator) 
        : base(gatherer, navAgent, animator) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.SpawnPoint;
    }
}