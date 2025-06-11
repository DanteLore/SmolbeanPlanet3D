using UnityEngine;
using UnityEngine.AI;

public class WalkHomeState : WalkStateBase
{
    private SmolbeanColonist colonist;

    public WalkHomeState(SmolbeanColonist colonist, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(colonist, navAgent, animator, soundPlayer) 
    {
        this.colonist = colonist;
    }

    protected override Vector3 GetDestination()
    {
        return colonist.Job.Building.spawnPoint.transform.position;
    }
}