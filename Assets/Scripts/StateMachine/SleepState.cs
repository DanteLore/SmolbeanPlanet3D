using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState : IState
{
    private ResourceGatherer gatherer;

    private float sleepStartTime;

    public float TimeAsleep { get { return Time.time - sleepStartTime; } }

    public SleepState(ResourceGatherer gatherer)
    {
        this.gatherer = gatherer;
    }

    public void OnEnter()
    {
        sleepStartTime = Time.time;
        gatherer.Hide();
    }

    public void OnExit()
    {
        gatherer.Show();
    }

    public void Tick()
    {
    }
}
