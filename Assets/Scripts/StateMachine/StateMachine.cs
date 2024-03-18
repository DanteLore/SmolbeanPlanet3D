using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    public bool shouldLog = false;
    private readonly Dictionary<Type, List<Transition>> transitions = new();
    private List<Transition> currentTransitions = new();

    private readonly List<Transition> anyTransitions = new();
    private static readonly List<Transition> EmptyTransitions = new(0); // Need this?
    private IState startState;

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

    public StateMachine(bool shouldLog = false)
    {
        this.shouldLog = shouldLog;
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
        if(state == currentState)
            return;

        Log("Changed state to: " + state.GetType().Name);

        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
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
        if(!transitions.TryGetValue(from.GetType(), out var trans))
        {
            trans = new List<Transition>();
            transitions[from.GetType()] = trans;
        }

        trans.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(IState to, Func<bool> predicate)
    {
        anyTransitions.Add(new Transition(to, predicate));
    }

    private Transition GetTransition()
    {
        // Quicker to do .ToArray() then unumerate, than to enumerate the list!
        foreach (var t in anyTransitions.ToArray()) 
            if(t.Condition())
                return t;

        foreach(var t in currentTransitions.ToArray())
            if(t.Condition())
                return t;
        
        return null;
    }

    private void Log(string message)
    {
        if (shouldLog)
            Debug.Log(message);
    }

    public void ForceStop()
    {
        currentState?.OnExit();
    }
}
