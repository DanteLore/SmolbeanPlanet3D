using UnityEngine;

public class EatGrassState : IState
{
    private readonly SmolbeanAnimal animal;

    public EatGrassState(SmolbeanAnimal animal)
    {
        this.animal = animal;
    }

    public void OnEnter()
    {
        animal.Think("Eating some grass...");
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        var pos = animal.transform.position;
        var available = GroundWearManager.Instance.GetAvailableGrass(pos);
        animal.Eat(animal.Species.foodEatenPerSecond * available * Time.deltaTime);
        GroundWearManager.Instance.RegisterHarvest(pos, animal.Species.grassWearPerSecondWhenEating * Time.deltaTime);
    }
}
