using UnityEngine;
using UnityEngine.AI;

public class WalkToDropState : WalkStateBase
{
    private ResourceGatherer gatherer;

    public WalkToDropState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(navAgent, animator, soundPlayer)
    {
        this.gatherer = gatherer;   
    }

    protected override Vector3 GetDestination()
    {
        return gatherer.DropTarget.transform.position;
    }
}
