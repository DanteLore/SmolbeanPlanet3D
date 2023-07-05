using UnityEngine;
using UnityEngine.AI;

public class WalkToDropPointState : WalkStateBase
{
    private ResourceGatherer gatherer;
    
    public WalkToDropPointState(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(navAgent, animator, soundPlayer)
    {
        this.gatherer = gatherer;
    }

    protected override Vector3 GetDestination()
    {
        return gatherer.DropPoint;
    }
}
