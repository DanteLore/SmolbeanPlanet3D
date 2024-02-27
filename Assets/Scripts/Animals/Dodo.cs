using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dodo : SmolbeanAnimal
{
    private float lastEggLaidTime = 0f;

    protected override void Start()
    {
        base.Start();

        target = transform.position; // We start where we want to be

        //var idle = new IdleState(animator);
        var graze = new GrazeState(this, animator, navAgent, soundPlayer);
        var flock = new FlockState(this, animator, navAgent, soundPlayer);

        AT(flock, graze, Hungry());
        AT(graze, flock, Full());
        
        stateMachine.SetState(flock);

        Func<bool> Hungry() => IsHungry;
        Func<bool> Full() => IsFull;
    }

    public override bool IsEnoughFoodHere()
    {
        return GroundWearManager.Instance.GetAvailableGrass(transform.position) >= 0.1f;
    }

    protected override void Update()
    {
        base.Update();

        if(age >= species.maturityAgeSeconds &&
            health >= species.minimumHealthToGiveBirth &&
            Time.time - lastEggLaidTime >= species.gestationPeriodSeconds &&
            Random.Range(0f, 1f) <= species.birthProbability * Time.deltaTime &&
            AnimalController.Instance.AnimalCount(species) < species.populationCap)
        {
            LayAnEgg();
        }
    }

    private void LayAnEgg()
    {
        lastEggLaidTime = Time.time;
        Instantiate(species.eggLaidParticleSystem, transform.position, Quaternion.Euler(0f, 0f, 0f));
        var egg = DropController.Instance.Drop(species.eggDropSpec, transform.position);
        egg.GetComponent<SmolbeanEgg>().species = species;
    }
}
