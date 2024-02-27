using UnityEngine;

public class SmolbeanEgg : SmolbeanDrop
{
    public AnimalSpec species;

    protected override void PerSecondUpdate(float age)
    {
        if (Time.time - createTime > species.chickGestationSeconds)
        {
            AnimalController.Instance.CreateAnimal(species, transform.position);
            Instantiate(species.eggLaidParticleSystem, transform.position, Quaternion.Euler(0f, 0f, 0f));

            Destroy(gameObject);
        }

        base.PerSecondUpdate(age);
    }
}
