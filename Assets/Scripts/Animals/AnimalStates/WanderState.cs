using UnityEngine;
using UnityEngine.AI;

public class WanderState : WalkStateBase
{
    private readonly SmolbeanAnimal animal;

    public WanderState(SmolbeanAnimal animal, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
        : base(navAgent, animator, soundPlayer)
    {
        this.animal = animal;

        // Disable the navagent reset, bec ause it's expensive to use in animals
        navAgentResetEnabled = false;
    }
    
    protected override Vector3 GetDestination()
    {
        return animal.target;
    }
}
