using UnityEngine;
using UnityEngine.AI;

public class WalkToTargetState : WalkStateBase
{
    private readonly SmolbeanAnimal animal;

    public WalkToTargetState(SmolbeanAnimal animal, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
        : base(animal, navAgent, animator, soundPlayer)
    {
        this.animal = animal;

        // Disable the navagent reset, because it's expensive to use in animals
        navAgentResetEnabled = false;
    }
    
    protected override Vector3 GetDestination()
    {
        return animal.Target;
    }

    protected override void OnStuck()
    {
        animal.Think("I think I'm stuck!");
    }
}
