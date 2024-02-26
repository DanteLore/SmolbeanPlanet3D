using System;
using UnityEngine;
using UnityEngine.AI;

public class FlockState : IState
{
    private readonly StateMachine stateMachine;
    private readonly IState startState;

    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    public FlockState(SmolbeanAnimal animal, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        stateMachine = new StateMachine(shouldLog: false);

        var idle = new IdleState(animator);
        var findWoodland = new ChooseFlockLocation(animal);
        var wander = new WanderState(animal, navAgent, animator, soundPlayer);

        AT(wander, HasSomewhereToGo());
        AT(wander, idle, Arrived());

        AT(idle, findWoodland, IdleFor(1f));

        // In case the search states find a very close point
        AT(findWoodland, idle, Arrived());

        startState = idle;
        stateMachine.SetState(idle);

        Func<bool> IdleFor(float idleTime) => () => idle.TimeIdle >= idleTime;
        Func<bool> HasSomewhereToGo() => () => !animal.CloseEnoughTo(animal.target);
        Func<bool> Arrived() => () => animal.CloseEnoughTo(animal.target);
    }

    public void OnEnter()
    {
        stateMachine.SetState(startState);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        stateMachine?.Tick();
    }
}
