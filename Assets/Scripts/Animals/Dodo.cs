using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dodo : SmolbeanAnimal
{
    protected override void Start()
    {
        base.Start();

        Target = transform.position; // We start where we want to be
        float sleepLightLevel = Random.Range(0.3f, 0.5f); // The light level we like to sleep at

        float idleSeconds = Random.Range(1f, 5f);
        float startTime = Time.time + idleSeconds;

        var sleep = new SleepState(this, animator, soundPlayer);
        var graze = new GrazeState(this, animator, navAgent, soundPlayer);
        var flock = new FlockState(this, animator, navAgent, soundPlayer);
        var startState = new GenericState();

        AT(startState, flock, TimeToStart());
        AT(flock, sleep, IsNight());
        AT(flock, graze, Hungry());
        AT(graze, sleep, IsNight());
        AT(graze, flock, Full());
        AT(sleep, flock, WakeupTime(5f));

        StateMachine.SetStartState(startState);

        Func<bool> Hungry() => IsHungry;
        Func<bool> Full() => IsFull;
        Func<bool> IsNight() => () => DayNightCycleController.Instance.LightLevel < sleepLightLevel;
        Func<bool> WakeupTime(float sleepMax) => () => DayNightCycleController.Instance.LightLevel > sleepLightLevel && sleep.SleepDuration >= sleepMax;
        Func<bool> TimeToStart() => () => Time.time > startTime;
    }

    public override bool IsEnoughFoodHere()
    {
        return GroundWearManager.Instance.GetAvailableGrass(transform.position) >= 0.1f;
    }

    protected override void Update()
    {
        base.Update();

        if(!stats.isDead &&
            stats.age >= Species.maturityAgeSeconds &&
            stats.health >= Species.minimumHealthToGiveBirth &&
            Time.time - ((DodoStats)stats).lastEggLaidTime >= Species.gestationPeriodSeconds &&
            Random.Range(0f, 1f) <= Species.birthProbability * Time.deltaTime &&
            AnimalController.Instance.AnimalCount(Species) < Species.populationCap)
        {
            LayAnEgg();
        }
    }

    private void LayAnEgg()
    {
        var birdSpecies = (BirdSpec)Species;
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
                age = Random.Range(0f, Species.lifespanSeconds),
                health = Species.initialHealth,
                foodLevel = Random.Range(Species.initialFoodLevelMin, Species.initialFoodLevelMax),
                lastEggLaidTime = 0.0f,
                scale = 1.0f
            };
        }
    }

    protected override void Die()
    {
        soundPlayer.PlayOneShot("Dodo3");
        base.Die();
    }
}
