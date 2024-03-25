using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public abstract class SmolbeanAnimal : MonoBehaviour
{
    protected void AT(IState from, IState to, Func<bool> condition) => StateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => StateMachine.AddAnyTransition(to, condition);

    public string natureLayer = "Nature";
    public string creatureLayer = "Creatures";
    public string groundLayer = "Ground";
    public string buildingLayer = "Buildings";
    public string dropLayer = "Drops";

    protected static readonly float destinationThreshold = 2.0f;

    public Inventory Inventory { get; private set; }

    protected AnimalStats stats;
    public AnimalStats Stats
    {
        get { return stats; }
    }

    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;

    protected StateMachine StateMachine { get; private set; }
    public int speciesIndex;
    public AnimalSpec species;
    public Vector3 target;
    protected GameObject body;
    protected bool isDead = false;
    protected bool isSleeping = false;
    private GameObject sleepPs;
    private readonly List<Thought> thoughts = new();
    public IEnumerable<Thought> Thoughts { get { return thoughts; } }
    public int memoryLength = 12;
    public EventHandler ThoughtsChanged;
    public int prefabIndex = -1;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        soundPlayer = GetComponent<SoundPlayer>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;

        Inventory = new Inventory();
        StateMachine = new StateMachine(shouldLog: false);
    }

    protected virtual void Update()
    {
        if (!isDead && !GameStateManager.Instance.IsPaused)
        {
            UpdateStats();

            if (stats.health <= 0f)
                return;

            StateMachine.Tick();
        }
    }

    public virtual void AdoptIdentity(SmolbeanAnimal original)
    {
        // Might be more to do here?
        stats = original.stats;
        species = original.species;
        thoughts.Clear();
        thoughts.AddRange(original.Thoughts);
    }

    public void Think(string thought)
    {
        while (thoughts.Count > memoryLength)
            thoughts.RemoveAt(0);

        var t = new Thought
        {
            thought = thought,
            day = DayNightCycleController.Instance.day,
            timeOfDay = DayNightCycleController.Instance.timeOfDay
        };

        thoughts.Add(t);

        ThoughtsChanged?.Invoke(this, EventArgs.Empty);
    }

    public abstract void InitialiseStats(AnimalStats newStats = null);

    protected virtual void UpdateStats()
    {
        stats.age += Time.deltaTime;

        float ageFactor = Mathf.InverseLerp(species.oldAgeSeconds, species.lifespanSeconds, stats.age);

        // Juveniles are small
        if(stats.age <= species.maturityAgeSeconds)
        {
            var x = Mathf.InverseLerp(0f, species.maturityAgeSeconds, stats.age);
            var s = Mathf.Lerp(species.juvenileScale, 1f, x);
            transform.localScale = new Vector3(s, s, s);
        }

        // Speed decrease due to old age
        navAgent.speed = species.speed - species.oldAgeSpeedDecrease * ageFactor;

        // Decreasing health due to old age
        float oldAgeHealthDetriment = species.oldAgeHealthImpactPerSecond * ageFactor * Time.deltaTime;
        if (isSleeping) // Less if sleeping!
            oldAgeHealthDetriment *= species.sleepingHealthDecreaseMultiplier;
        stats.health -= oldAgeHealthDetriment;

        // Digest some food
        float foodDelta = species.foodDigestedPerSecond * Time.deltaTime;
        if (isSleeping) // Less if sleeping!
            foodDelta *= species.sleepingHealthDecreaseMultiplier;
        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - foodDelta);

        if (stats.foodLevel <= species.starvationThreshold)
        {
            // Health decrease due to starvation
            float healthDelta = species.starvationRatePerSecond * Time.deltaTime;
            healthDelta *= 1f - Mathf.InverseLerp(0f, species.starvationThreshold, stats.foodLevel);
            if (isSleeping) // Less if sleeping!
                healthDelta *= species.sleepingHealthDecreaseMultiplier;
            stats.health -= healthDelta; 
        }
        else
        {
            // Health recover with a full stomach
            float healthDelta = species.healthRecoveryPerSecond * ageFactor * Time.deltaTime;
            healthDelta *= Mathf.InverseLerp(species.starvationThreshold, species.maxFoodLevel, stats.foodLevel);
            stats.health = Mathf.Min(species.maxHealth, stats.health + healthDelta);
        }

        if (stats.health <= 0)
            Die(); 
    }

    public void LoadFrom(AnimalSaveData saveData)
    {
        speciesIndex = saveData.speciesIndex;
        prefabIndex = saveData.prefabIndex;
        thoughts.Clear();
        thoughts.AddRange(saveData.thoughts);
        InitialiseStats(saveData.stats);
    }

    protected virtual void Die()
    {
        isDead = true;
        StartCoroutine(DoDeathActivities());
    }

    private IEnumerator DoDeathActivities()
    {
        yield return new WaitForEndOfFrame();
        Instantiate(species.deathParticleSystem, transform.position, Quaternion.Euler(0f, 0f, 0f));
        //soundPlayer.Play("Break");
        
        DropController.Instance.Drop(species.dropSpec, transform.position);
        yield return new WaitForEndOfFrame();

        body.SetActive(false);
        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }

    public virtual float Eat(float amount)
    {
        float delta = Mathf.Min(amount, species.maxFoodLevel - stats.foodLevel);
        stats.foodLevel += delta;
        return delta;
    }

    public AnimalSaveData GetSaveData()
    {
        return new AnimalSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            rotationY = transform.rotation.eulerAngles.y,
            speciesIndex = speciesIndex,
            prefabIndex = prefabIndex,
            stats = stats,
            thoughts = Thoughts.ToArray()
        };
    }

    public void Hide()
    {
        body.SetActive(false);
    }

    public void Show()
    {
        body.SetActive(true);
    }

    public bool CloseEnoughTo(Vector3 dest)
    {
        var pos = transform.position;

        Vector3 v1 = new(pos.x, 0.0f, pos.z);
        Vector3 v2 = new(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold * destinationThreshold;
    }

    public bool CloseEnoughTo(GameObject target)
    {
        if (target == null)
            return false;

        // If we don't have a collider, just measure the distance
        if (target.GetComponentInChildren<Collider>() == null)
            return CloseEnoughTo(target.transform.position);

        // Otherwise...
        // This DOES need to be a sphere collision, rather than a standard distance check, because the object might be big and
        // it's potition might be a point right in it's centre.
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(LayerMask.LayerToName(target.layer)));
        return found.Any(c => c.gameObject == target);
    }

    public void StartSleep()
    {
        isSleeping = true;
        float y = GetComponentInChildren<Collider>().bounds.max.y * 1.1f;
        var animalPosition = transform.position;
        var p = new Vector3(animalPosition.x, y, animalPosition.z);
        sleepPs = Instantiate(species.sleepParticleSystem, p, Quaternion.Euler(0f, 0f, 0f), transform);
    }

    public void EndSleep()
    {
        isSleeping = false;
        if (sleepPs != null)
        {
            Destroy(sleepPs);
            sleepPs = null;
        }
    }

    public virtual bool IsEnoughFoodHere()
    {
        return false;
    }

    public virtual bool IsFull()
    {
        return stats.foodLevel > species.fullThreshold;
    }

    public virtual bool IsHungry()
    {
        return stats.foodLevel < species.hungryThreshold;
    }

    public void DropInventory()
    {
        var pos = transform.position;

        while (!Inventory.IsEmpty())
        {
            var item = Inventory.DropLast();

            Vector3 upPos = Vector3.up * 1f;
            Vector3 outPos = Vector3.left * Random.Range(0f, 1f);
            outPos = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * outPos;

            DropController.Instance.Drop(item.dropSpec, pos + upPos + outPos, item.quantity);
        }
    }
}