using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine
{
    public bool ShouldLog = false;
    public Action<string> OnLogMessage;

    private IState currentState;
    private readonly bool allowSelfTransitions;
    private readonly Dictionary<IState, Transition[]> transitions = new();
    private Transition[] currentTransitions = Array.Empty<Transition>();

    private Transition[] anyTransitions = Array.Empty<Transition>();
    private static readonly Transition[] EmptyTransitions = Array.Empty<Transition>();
    private IState startState;

    public string CurrentStateName { get => currentState.GetType().Name; }

    private class Transition
    {
        public IState To { get; private set; }
        public Func<bool> Condition { get; private set; }
        
        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    public StateMachine(bool shouldLog = false, bool allowSelfTransitions = false)
    {
        this.ShouldLog = shouldLog;
        this.allowSelfTransitions = allowSelfTransitions;
    }

    public void Tick()
    {
        if (currentState == null)
        {
            Restart();
        }
        else
        {
            var transition = GetTransition();
            if (transition != null)
                SetState(transition.To);

            currentState?.Tick();
        }
    }

    public void Restart()
    {
        SetState(startState);
    }

    private void SetState(IState state)
    {
        if(!allowSelfTransitions && state == currentState)
            return;

        if (ShouldLog) // Check flag here to avoid needless string concat!
            Log("Changed state to: " + state.ToString());

        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState, out currentTransitions);
        if(currentTransitions == null)
            currentTransitions = EmptyTransitions;

        currentState?.OnEnter();
    }

    public void SetStartState(IState startState)
    {
        Debug.Assert(startState != null, "You can't set the start state to null!");
        this.startState = startState;
    }

    public void AddTransition(IState from, IState to, Func<bool> predicate)
    {
        if(!transitions.TryGetValue(from, out var trans))
            trans = Array.Empty<Transition>();

        transitions[from] = trans.Append(new Transition(to, predicate)).ToArray();
    }

    public void AddAnyTransition(IState to, Func<bool> predicate)
    {
        anyTransitions = anyTransitions.Append(new Transition(to, predicate)).ToArray();
    }

    private Transition GetTransition()
    {
        foreach (var t in anyTransitions) 
            if(t.Condition())
                return t;

        foreach(var t in currentTransitions)
            if(t.Condition())
                return t;
        
        return null;
    }

    private void Log(string message)
    {
        if (ShouldLog)
        {          
            OnLogMessage?.Invoke(message);
            Debug.Log(message);
        }
    }

    public void ForceStop()
    {
        currentState?.OnExit();
    }
}
