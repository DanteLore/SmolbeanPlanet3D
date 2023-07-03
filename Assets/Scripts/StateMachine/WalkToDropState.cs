using UnityEngine;
using UnityEngine.AI;

public class WalkToDropState : WalkStateBase
{
    public WalkToDropState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(gatherer, navAgent, animator, soundPlayer) {}

    protected override Vector3 GetDestination()
    {
        return gatherer.DropTarget.transform.position;
    }
}
