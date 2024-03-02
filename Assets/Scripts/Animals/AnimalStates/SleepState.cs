using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepState : IState
{
    private float sleepStartTime;
    private readonly SmolbeanAnimal animal;
    private readonly Animator animator;
    private readonly SoundPlayer soundPlayer;

    public SleepState(SmolbeanAnimal animal, Animator animator, SoundPlayer soundPlayer)
    {
        this.animal = animal;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
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
