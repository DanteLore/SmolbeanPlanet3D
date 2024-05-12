using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FreeColonistState : CompoundState
{
    public FreeColonistState(SmolbeanColonist colonist, Animator animator, UnityEngine.AI.NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        var idle = new IdleState(animator);
        var wander = new WalkToTargetState(colonist, navAgent, animator, soundPlayer);
        var findRestingPlace = new FindRestingPlaceState(colonist);

        AT(idle, findRestingPlace, IdleForUpTo(20f));
        AT(findRestingPlace, wander, HasSomewhereToGo());
        AT(wander, idle, Arrived());
        AT(wander, findRestingPlace, Stuck());

        stateMachine.SetStartState(idle);

        Func<bool> HasSomewhereToGo() => () => !colonist.CloseEnoughTo(colonist.target, 0.5f);
        Func<bool> Arrived() => () => colonist.CloseEnoughTo(colonist.target, 2f);
        Func<bool> IdleForUpTo(float seconds) => () => idle.TimeIdle >= Random.Range(0f, seconds);
        Func<bool> Stuck() => () => wander.IsStuck;
    }
}
