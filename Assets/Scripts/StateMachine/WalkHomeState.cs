using UnityEngine;
using UnityEngine.AI;

public class WalkHomeState : WalkStateBase
{
    public WalkHomeState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(gatherer, navAgent, animator, soundPlayer) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.SpawnPoint;
    }
}