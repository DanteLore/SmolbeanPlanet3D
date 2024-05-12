using System;
using UnityEngine;
using UnityEngine.AI;

public class FlockState : CompoundState
{
    private readonly SoundPlayer soundPlayer;

    public FlockState(SmolbeanAnimal animal, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        var idle = new IdleState(animator);
        var findDestination = new ChooseFlockLocation(animal);
        var wander = new WalkToTargetState(animal, navAgent, animator, soundPlayer);

        AT(wander, HasSomewhereToGo());
        AT(wander, idle, Arrived());
        AT(wander, findDestination, Stuck());

        AT(idle, findDestination, IdleFor(1f));

        // In case the search states find a very close point
        AT(findDestination, idle, Arrived());

        stateMachine.SetStartState(idle);

        Func<bool> IdleFor(float idleTime) => () => idle.TimeIdle >= idleTime;
        Func<bool> HasSomewhereToGo() => () => !animal.CloseEnoughTo(animal.target, 1f);
        Func<bool> Arrived() => () => animal.CloseEnoughTo(animal.target, 2f);
        Func<bool> Stuck() => () => wander.IsStuck;
        this.soundPlayer = soundPlayer;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        soundPlayer.PlayOneShot("Dodo1");        
    }
}
