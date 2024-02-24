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
        var pos = animal.transform.position;
        float desired = animal.species.foodEatenPerSecond * Time.deltaTime;
        animal.Eat(desired);
        GroundWearManager.Instance.RegisterHarvest(pos, 0.5f);
    }
}
