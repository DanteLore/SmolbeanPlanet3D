using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private IState currentState;
    private bool shouldLog = false;
    private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> currentTransitions = new List<Transition>();
    private List<Transition> anyTransitions = new List<Transition>();
    private static List<Transition> EmptyTransitions = new List<Transition>(0); // Need this?

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

    private void Log(string message)
    {
        if(shouldLog)
            Debug.Log(message);
    }

    public void Tick()
    {
        var transition = GetTransition();
        if(transition != null)
            SetState(transition.To);

        currentState?.Tick();
    }

    public void SetState(IState state)
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
}
