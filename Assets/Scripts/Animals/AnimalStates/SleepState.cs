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
        animal.Think("ZZZZZzzzzz....");
        sleepStartTime = Time.time;
        animal.StartSleep();
    }

    public void OnExit()
    {
        animal.EndSleep();
        animal.Think("Time to get up *yawn*");
    }

    public void Tick()
    {
    }
}
