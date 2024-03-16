using System;
using UnityEngine;

public class DoJobState : CompoundState
{
    public DoJobState(SmolbeanColonist colonist, Animator animator, UnityEngine.AI.NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        var idle = new IdleState(animator);
        var wander = new WalkToTargetState(colonist, navAgent, animator, soundPlayer);
        var goToWork = new GoToWorkState(colonist);
        var switchColonist = new SwitchColonistState(colonist);

        AT(idle, goToWork, IdleFor(1f));
        AT(goToWork, wander, HasSomewhereToGo());
        AT(wander, switchColonist, Arrived());

        stateMachine.SetStartState(idle);

        Func<bool> HasSomewhereToGo() => () => !colonist.CloseEnoughTo(colonist.target);
        Func<bool> Arrived() => () => colonist.CloseEnoughTo(colonist.target);
        Func<bool> IdleFor(float seconds) => () => idle.TimeIdle >= seconds;
    }
}
