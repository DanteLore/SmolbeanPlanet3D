using UnityEngine;
using UnityEngine.AI;

public class WalkToDropPointState : WalkStateBase
{
    private IReturnDrops colonist;
    
    public WalkToDropPointState(IReturnDrops colonist, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base((SmolbeanAnimal)colonist, navAgent, animator, soundPlayer)
    {
        this.colonist = colonist;
    }

    protected override Vector3 GetDestination()
    {
        return colonist.Job.Building.dropPoint.transform.position;
    }
}
