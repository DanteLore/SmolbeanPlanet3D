using System;
using UnityEngine;

public class FreeColonistState : CompoundState
{
    public FreeColonistState(SmolbeanColonist colonist, Animator animator, UnityEngine.AI.NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        var idle = new IdleState(animator);
        var wander = new WalkToTargetState(colonist, navAgent, animator, soundPlayer);
        var findRestingPlace = new FindRestingPlaceState(colonist);

        AT(idle, findRestingPlace, IdleFor(20f));
        AT(findRestingPlace, wander, HasSomewhereToGo());
        AT(wander, idle, Arrived());

        stateMachine.SetStartState(idle);

        Func<bool> HasSomewhereToGo() => () => !colonist.CloseEnoughTo(colonist.target);
        Func<bool> Arrived() => () => colonist.CloseEnoughTo(colonist.target);
        Func<bool> IdleFor(float seconds) => () => idle.TimeIdle >= seconds;
    }
}
