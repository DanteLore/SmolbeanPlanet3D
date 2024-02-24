using UnityEngine;

public class EatGrassState : IState
{
    private SmolbeanAnimal animal;

    private float sleepStartTime;

    public float TimeAsleep { get { return Time.time - sleepStartTime; } }

    public EatGrassState(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        sleepStartTime = Time.time;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
