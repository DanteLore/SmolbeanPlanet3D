using UnityEngine;
using UnityEngine.AI;

public class WalkToDropPointState : WalkStateBase
{
    public WalkToDropPointState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(gatherer, navAgent, animator, soundPlayer) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.DropPoint;
    }
}
