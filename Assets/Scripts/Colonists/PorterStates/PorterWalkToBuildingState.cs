using UnityEngine;
using UnityEngine.AI;

public class PorterWalkToBuildingState : WalkStateBase
{
    private Porter porter;

    public PorterWalkToBuildingState(Porter porter, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer) 
        : base(porter, navAgent, animator, soundPlayer) 
    {
        this.porter = porter;
    }

    protected override Vector3 GetDestination()
    {
        return porter.DeliveryRequest.Building.GetSpawnPoint();
    }
}
