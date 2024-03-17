using System;
using UnityEngine;

public class DoJobState : CompoundState
{
    public DoJobState(SmolbeanColonist colonist, Animator animator, UnityEngine.AI.NavMeshAgent navAgent, SoundPlayer soundPlayer)
    {
        var wander = new WalkToTargetState(colonist, navAgent, animator, soundPlayer);
        var goToWork = new GoToWorkState(colonist);
        var switchColonist = new SwitchColonistToProfessionState(colonist);

        AT(goToWork, wander, HasSomewhereToGo());
        AT(wander, switchColonist, Arrived());

        stateMachine.SetStartState(goToWork);

        Func<bool> HasSomewhereToGo() => () => !colonist.CloseEnoughTo(colonist.target);
        Func<bool> Arrived() => () => colonist.CloseEnoughTo(colonist.target);
    }
}
