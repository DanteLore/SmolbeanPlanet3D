using System;
using UnityEngine;
using UnityEngine.AI;

public class GrazeState : CompoundState
{
    private readonly SoundPlayer soundPlayer;

    public GrazeState(SmolbeanAnimal animal, Animator animator, NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        var eat = new EatGrassState(animal);
        var lookForPlaceToEat = new ChooseEatingPlaceState(animal);
        var wander = new WalkToTargetState(animal, navAgent, animator, soundPlayer);

        AT(wander, HasSomewhereToGo());
        AT(wander, eat, Arrived());
        AT(wander, lookForPlaceToEat, Stuck());

        AT(eat, lookForPlaceToEat, NotEnoughFoodHere());

        // In case the search states find a very close point
        AT(lookForPlaceToEat, eat, Arrived());

        stateMachine.SetStartState(eat);

        Func<bool> HasSomewhereToGo() => () => !animal.CloseEnoughTo(animal.Target, 0.5f);
        Func<bool> Arrived() => () => animal.CloseEnoughTo(animal.Target, 1f);
        Func<bool> NotEnoughFoodHere() => () => !animal.IsEnoughFoodHere();
        Func<bool> Stuck() => () => wander.IsStuck;
        this.soundPlayer = soundPlayer;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        soundPlayer.PlayOneShot("Dodo2");
    }
}
