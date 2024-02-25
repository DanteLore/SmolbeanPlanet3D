using System;
using UnityEngine;
using UnityEngine.AI;

public class GrazeState : IState
{
    private readonly StateMachine stateMachine;
    private readonly IState startState;

    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    public GrazeState(SmolbeanAnimal animal, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        stateMachine = new StateMachine(shouldLog: false);

        var eat = new EatGrassState(animal);
        var lookForPlaceToEat = new ChooseEatingPlaceState(animal);
        var wander = new WanderState(animal, navAgent, animator, soundPlayer);

        AT(wander, HasSomewhereToGo());
        AT(wander, eat, Arrived());

        AT(eat, lookForPlaceToEat, NotEnoughFoodHere());

        // In case the search states find a very close point
        AT(lookForPlaceToEat, eat, Arrived());

        startState = eat;
        stateMachine.SetState(eat);

        Func<bool> HasSomewhereToGo() => () => !animal.CloseEnoughTo(animal.target);
        Func<bool> Arrived() => () => animal.CloseEnoughTo(animal.target);
        Func<bool> NotEnoughFoodHere() => () => !animal.IsEnoughFoodHere();
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
