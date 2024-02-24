using UnityEngine;
using UnityEngine.AI;

public class WalkToDropState : WalkStateBase
{
    private IGatherDrops gatherer;

    public WalkToDropState(IGatherDrops gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(navAgent, animator, soundPlayer)
    {
        this.gatherer = gatherer;   
    }

    protected override Vector3 GetDestination()
    {
        return gatherer.TargetDrop.transform.position;
    }
}
