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

    [SerializeField] protected string natureLayer = "Nature";
    [SerializeField] protected string creatureLayer = "Creatures";
    [SerializeField] protected string groundLayer = "Ground";
    [SerializeField] protected string buildingLayer = "Buildings";
    [SerializeField] protected string dropLayer = "Drops";
    [SerializeField] protected int memoryLength = 12;

    private readonly List<Thought> thoughts = new();
    private GameObject sleepPs;

    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;
    protected AnimalStats stats;
    protected GameObject body;
    protected bool isDead = false;
    protected bool isSleeping = false;
    protected StateMachine StateMachine { get; private set; }

    public int SpeciesIndex { get; set; }
    public AnimalSpec Species { get; set; }
    public Vector3 Target { get; set; }
    public int PrefabIndex { get; set; }

    public Inventory Inventory { get; private set; }
    public AnimalStats Stats { get { return stats; } }
    public IEnumerable<Thought> Thoughts { get { return thoughts; } }

    public EventHandler ThoughtsChanged;

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
        Species = original.Species;
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
            day = DayNightCycleController.Instance.Day,
            timeOfDay = DayNightCycleController.Instance.TimeOfDay
        };

        thoughts.Add(t);

        ThoughtsChanged?.Invoke(this, EventArgs.Empty);
    }

    public abstract void InitialiseStats(AnimalStats newStats = null);

    protected virtual void UpdateStats()
    {
        stats.age += Time.deltaTime;

        AnimalSpec species = Species; // Cache for miniscule performance boost!

        float ageFactor = (stats.age < species.oldAgeSeconds) ? 0.0f : Mathf.InverseLerp(species.oldAgeSeconds, species.lifespanSeconds, stats.age);

        // Juveniles are small
        if(stats.age <= species.maturityAgeSeconds)
        {
            var x = Mathf.InverseLerp(0f, species.maturityAgeSeconds, stats.age);
            var s = Mathf.Lerp(species.juvenileScale, 1f, x);
            transform.localScale = new Vector3(s, s, s);
        }

        // Speed decrease due to old age
        if(ageFactor > 0.0f)
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
            float healthDelta = species.healthRecoveryPerSecond * Time.deltaTime;
            healthDelta *= Mathf.InverseLerp(species.starvationThreshold, species.maxFoodLevel, stats.foodLevel);
            stats.health = Mathf.Min(species.maxHealth, stats.health + healthDelta);
        }

        if (stats.health <= 0)
            Die(); 
    }

    public virtual void LoadFrom(AnimalSaveData saveData)
    {
        SpeciesIndex = saveData.speciesIndex;
        PrefabIndex = saveData.prefabIndex;
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
        Instantiate(Species.deathParticleSystem, transform.position, Quaternion.Euler(0f, 0f, 0f));
        
        // Only drop a steak if we didn't starve to death and aren't too old
        if(stats.foodLevel > Species.starvationThreshold && stats.age < Species.lifespanSeconds)
            DropController.Instance.Drop(Species.dropSpec, transform.position);

        yield return new WaitForEndOfFrame();

        body.SetActive(false);
        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }

    public virtual float Eat(float amount)
    {
        float delta = Mathf.Min(amount, Species.maxFoodLevel - stats.foodLevel);
        stats.foodLevel += delta;
        return delta;
    }

    public virtual AnimalSaveData GetSaveData()
    {
        return new AnimalSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            rotationY = transform.rotation.eulerAngles.y,
            speciesIndex = SpeciesIndex,
            prefabIndex = PrefabIndex,
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

    public bool CloseEnoughTo(Vector3 dest, float destinationThreshold)
    {
        var pos = transform.position;

        Vector2 v1 = new(pos.x,  pos.z);
        Vector2 v2 = new(dest.x, dest.z);

        return Vector2.SqrMagnitude(v1 - v2) <= destinationThreshold * destinationThreshold;
    }

    public bool CloseEnoughTo(GameObject target, float destinationThreshold)
    {
        if (target == null)
            return false;

        var collider = target.GetComponentInChildren<Collider>();

        // If we don't have a collider, just measure the distance to the object position
        if (collider == null)
            return CloseEnoughTo(target.transform.position, destinationThreshold);

        // If we do have a collider, measure the distance to the closest point on it!
        var pos = transform.position;
        return Vector3.SqrMagnitude(pos - collider.ClosestPoint(pos)) <= destinationThreshold * destinationThreshold;
    }

    public void StartSleep()
    {
        isSleeping = true;
        float y = GetComponentInChildren<Collider>().bounds.max.y * 1.1f;
        var animalPosition = transform.position;
        var p = new Vector3(animalPosition.x, y, animalPosition.z);
        sleepPs = Instantiate(Species.sleepParticleSystem, p, Quaternion.Euler(0f, 0f, 0f), transform);
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
        return stats.foodLevel > Species.fullThreshold;
    }

    public virtual bool IsHungry()
    {
        return stats.foodLevel < Species.hungryThreshold;
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

    public void TakeDamage(float damage)
    {
        Stats.health -= damage;
    }
}