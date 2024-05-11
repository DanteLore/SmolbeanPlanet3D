using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        target = transform.position; // We start where we want to be
        float sleepLightLevel = Random.Range(0.3f, 0.5f); // The light level we like to sleep at

        var sleep = new SleepState(this, animator, soundPlayer);
        var graze = new GrazeState(this, animator, navAgent, soundPlayer);
        var flock = new FlockState(this, animator, navAgent, soundPlayer);

        AT(flock, sleep, IsNight());
        AT(flock, graze, Hungry());
        AT(graze, sleep, IsNight());
        AT(graze, flock, Full());
        AT(sleep, flock, WakeupTime(5f));
        
        StateMachine.SetStartState(flock);

        Func<bool> Hungry() => IsHungry;
        Func<bool> Full() => IsFull;
        Func<bool> IsNight() => () => DayNightCycleController.Instance.LightLevel < sleepLightLevel;
        Func<bool> WakeupTime(float sleepMax) => () => DayNightCycleController.Instance.LightLevel > sleepLightLevel && sleep.SleepDuration >= sleepMax;
    }

    public override bool IsEnoughFoodHere()
    {
        return GroundWearManager.Instance.GetAvailableGrass(transform.position) >= 0.1f;
    }

    protected override void Update()
    {
        base.Update();

        if(!isDead &&
            stats.age >= species.maturityAgeSeconds &&
            stats.health >= species.minimumHealthToGiveBirth &&
            Time.time - ((DodoStats)stats).lastEggLaidTime >= species.gestationPeriodSeconds &&
            Random.Range(0f, 1f) <= species.birthProbability * Time.deltaTime &&
            AnimalController.Instance.AnimalCount(species) < species.populationCap)
        {
            LayAnEgg();
        }
    }

    private void LayAnEgg()
    {
        var birdSpecies = (BirdSpec)species;
        ((DodoStats)stats).lastEggLaidTime = Time.time;
        stats.health -= birdSpecies.pregnancyHealthImpact;
        Instantiate(birdSpecies.eggLaidParticleSystem, transform.position, Quaternion.Euler(0f, 0f, 0f));
        var egg = DropController.Instance.Drop(birdSpecies.eggDropSpec, transform.position);
        egg.GetComponent<SmolbeanEgg>().species = birdSpecies;
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        if (newStats != null)
        {
            stats = newStats;
        }
        else
        {
            stats = new DodoStats()
            {
                name = DodoNameGenerator.Generate(),
                age = Random.Range(0f, species.lifespanSeconds),
                health = species.initialHealth,
                foodLevel = Random.Range(species.initialFoodLevelMin, species.initialFoodLevelMax),
                lastEggLaidTime = 0f
            };
        }
    }

    protected override void Die()
    {
        soundPlayer.PlayOneShot("Dodo3");
        base.Die();
    }
}
