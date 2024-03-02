using System;
using UnityEngine;

public class SmolbeanEgg : SmolbeanDrop
{
    public BirdSpec species;

    protected override void PerSecondUpdate(float age)
    {
        if (Time.time - createTime > species.chickGestationSeconds)
            Hatch();

        base.PerSecondUpdate(age);
    }

    private void Hatch()
    {
        AnimalController.Instance.CreateAnimal(species, transform.position);
        Instantiate(species.eggLaidParticleSystem, transform.position, Quaternion.Euler(0f, 0f, 0f));

        Destroy(gameObject);
    }

    public override DropItemSaveData GetSaveData()
    {
        return new EggDropSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            dropSpecName = dropSpec.dropName,
            quantity = quantity,
            speciesIndex = Array.IndexOf(AnimalController.Instance.animalSpecs, species)
        };
    }

    public override void LoadExtraData(DropItemSaveData saveData)
    {
        if (saveData is not EggDropSaveData eggSpec)
            throw new Exception("Failed to load extra egg data");

        species = AnimalController.Instance.animalSpecs[eggSpec.speciesIndex] as BirdSpec;
    }
}
