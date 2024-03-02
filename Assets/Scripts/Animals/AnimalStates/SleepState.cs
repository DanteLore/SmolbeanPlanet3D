using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState : IState
{
    private float sleepStartTime;
    private readonly SmolbeanAnimal animal;

    public SleepState(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public float SleepDuration { get { return Time.time - sleepStartTime; } }

    public void OnEnter()
    {
        sleepStartTime = Time.time;
        animal.StartSleep();
    }

    public void OnExit()
    {
        animal.EndSleep();
    }

    public void Tick()
    {
    }
}
