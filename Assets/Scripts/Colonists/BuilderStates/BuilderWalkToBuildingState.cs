using UnityEngine;
using UnityEngine.AI;

public class BuilderWalkToBuildingState : WalkStateBase
{
    private Builder builder;

    public BuilderWalkToBuildingState(Builder builder, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(builder, navAgent, animator, soundPlayer) 
    {
        this.builder = builder;
    }

    protected override Vector3 GetDestination()
    {
        return builder.TargetBuilding.spawnPoint.transform.position;
    }
}
