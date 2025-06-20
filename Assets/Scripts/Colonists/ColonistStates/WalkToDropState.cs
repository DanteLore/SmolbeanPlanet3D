using UnityEngine;
using UnityEngine.AI;

public class WalkToDropState : WalkStateBase
{
    private readonly IGatherDrops gatherer;

    public WalkToDropState(IGatherDrops gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base((SmolbeanAnimal)gatherer, navAgent, animator, soundPlayer)
    {
        this.gatherer = gatherer;   
    }

    protected override Vector3 GetDestination()
    {
        return gatherer.TargetDrop.transform.position;
    }
}
