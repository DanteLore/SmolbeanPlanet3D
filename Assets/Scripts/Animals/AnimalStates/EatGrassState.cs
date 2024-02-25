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
        var available = GroundWearManager.Instance.GetAvailableGrass(pos);
        animal.Eat(animal.species.foodEatenPerSecond * available * Time.deltaTime);
        GroundWearManager.Instance.RegisterHarvest(pos, animal.species.grassWearPerSecondWhenEating * Time.deltaTime);
    }
}
