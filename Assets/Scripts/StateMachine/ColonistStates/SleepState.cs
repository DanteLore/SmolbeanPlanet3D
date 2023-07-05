using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState : IState
{
    private Colonist colonist;

    private float sleepStartTime;

    public float TimeAsleep { get { return Time.time - sleepStartTime; } }

    public SleepState(Colonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        sleepStartTime = Time.time;
        colonist.Hide();
    }

    public void OnExit()
    {
        colonist.Show();
    }

    public void Tick()
    {
    }
}
