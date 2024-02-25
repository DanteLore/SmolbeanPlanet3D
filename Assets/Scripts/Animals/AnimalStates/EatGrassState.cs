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
