using UnityEngine;

public class EatGrassState : IState
{
    private readonly SmolbeanAnimal animal;
    private float lastUpdateTime;

    public EatGrassState(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        animal.Think("Eating some grass...");
        lastUpdateTime = Time.time;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        // Can't use Time.deltaTime as we might not be called every frame
        float t = Time.time;
        float dt = t - lastUpdateTime;
        lastUpdateTime = t;

        var pos = animal.transform.position;
        var available = GroundWearManager.Instance.GetAvailableGrass(pos);  
        animal.Eat(animal.Species.foodEatenPerSecond * available * dt);
        GroundWearManager.Instance.RegisterHarvest(pos, animal.Species.grassWearPerSecondWhenEating * dt);
    }
}
