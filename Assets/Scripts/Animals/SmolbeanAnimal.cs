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

    private readonly List<BuffInstance> buffs = new();
    private readonly HashSet<string> buffNamesHash = new();
    public List<BuffInstance> Buffs { get { return buffs; } }

    public Transform transformCached;
    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;
    protected AnimalStats stats;
    protected GameObject body;
    protected StateMachine StateMachine { get; private set; }

    public int SpeciesIndex { get; set; }
    public AnimalSpec Species { get; set; }
    public Vector3 Target { get; set; }
    public int PrefabIndex { get; set; }

    public Inventory Inventory { get; private set; }
    public AnimalStats Stats { get { return stats; } }
    public IEnumerable<Thought> Thoughts { get { return thoughts; } }

    public EventHandler ThoughtsChanged;
    private Vector3 lastPosition;
    private float currentSpeed;
    private float currentScale;
    private readonly List<BuffInstance> newBuffs = new(capacity: 50);

    private void Awake()
    {
        transformCached = transform;
    }

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        soundPlayer = GetComponent<SoundPlayer>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transformCached.Find("Body").gameObject;

        Inventory = new Inventory();
        StateMachine = new StateMachine(shouldLog: false);
        lastPosition = transformCached.position;

        currentSpeed = navAgent.speed;
        currentScale = 1.0f;
    }

    protected virtual void Update()
    {
        if (!stats.isDead && !GameStateManager.Instance.IsPaused)
        {
            UpdateStats();

            if (stats.health <= 0f)
                return;

            StateMachine.Tick();
        }
    }

    public virtual void AdoptIdentity(SmolbeanAnimal original)
    {
        stats = original.stats;
        Species = original.Species;
        thoughts.Clear();
        thoughts.AddRange(original.Thoughts);
        buffs.Clear();
        buffs.AddRange(original.buffs);
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
        // Update tracker style stats
        var pos = transformCached.position;
        float dd = (lastPosition - pos).sqrMagnitude;
        if (dd > 1)
        {
            stats.distanceTravelled += Mathf.Sqrt(dd);
            lastPosition = pos;
        }

        // Apply the buffs
        ApplyBuffs();

        // Did we die?
        if (stats.health <= 0)
            Die();

        // Did we grow/shrink?
        if (stats.scale != currentScale)
        {
            transformCached.localScale = new Vector3(currentScale, currentScale, currentScale);
            currentScale = stats.scale;
        }

        // Did our speed change?
        if (currentSpeed != stats.speed)
        {
            navAgent.speed = stats.speed;
            currentSpeed = stats.speed;
        }
    }

    private void ApplyBuffs()
    {
        BuffsCleanup();

        // Apply all the buffs and append any new ones when we're done
        newBuffs.Clear();

        float dt = Time.deltaTime;

        foreach (var buff in buffs)
        {
            buff.ApplyTo(Stats, Species, dt, newBuffs);

            if (buff.GetThought(Stats, dt, out string thought))
                Think(thought);
        }

        // Add the buffs
        if(newBuffs.Count > 0)
            foreach (var buff in newBuffs)
                AddBuff(buff);
    }

    private void BuffsCleanup()
    {
        int i = buffs.Count - 1;
        while (i >= 0)
        {
            if (buffs[i].isExpired)
            {
                buffNamesHash.Remove(buffs[i].Spec.buffName);
                buffs.RemoveAt(i);
            }

            i--;
        }
    }

    private void AddBuff(BuffInstance buff)
    {
        if (buffNamesHash.Contains(buff.Spec.name))
            return;

        buffs.Add(buff);
        buffNamesHash.Add(buff.Spec.name);
    }

    protected virtual void Die()
    {
        stats.isDead = true;
        StartCoroutine(DoDeathActivities());
    }

    private IEnumerator DoDeathActivities()
    {
        yield return new WaitForEndOfFrame();
        Instantiate(Species.deathParticleSystem, transformCached.position, Quaternion.Euler(0f, 0f, 0f));
        
        // Only drop a steak if we didn't starve to death and aren't too old
        if(stats.foodLevel > Species.starvationThreshold && stats.age < Species.lifespanSeconds)
            DropController.Instance.Drop(Species.dropSpec, transformCached.position);

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

    public virtual void LoadFrom(AnimalSaveData saveData)
    {
        SpeciesIndex = saveData.speciesIndex;
        PrefabIndex = saveData.prefabIndex;
        thoughts.Clear();
        thoughts.AddRange(saveData.thoughts);
        InitialiseStats(saveData.stats);

        buffs.Clear();
        buffNamesHash.Clear();
        if (saveData.buffs != null)
        {
            foreach (var b in saveData.buffs)
            {
                // Reestablish the link to the BuffSpec
                b.Spec = BuffController.Instance.BuffSpecs[b.buffSpecIndex];
                // Add the buff
                AddBuff(b);
            }
        }
    }

    public virtual AnimalSaveData GetSaveData()
    {
        return new AnimalSaveData
        {
            positionX = transformCached.position.x,
            positionY = transformCached.position.y,
            positionZ = transformCached.position.z,
            rotationY = transformCached.rotation.eulerAngles.y,
            speciesIndex = SpeciesIndex,
            prefabIndex = PrefabIndex,
            stats = stats,
            thoughts = Thoughts.ToArray(),
            buffs = buffs.ToArray()
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
        var pos = transformCached.position;

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
        var pos = transformCached.position;
        return Vector3.SqrMagnitude(pos - collider.ClosestPoint(pos)) <= destinationThreshold * destinationThreshold;
    }

    public void StartSleep()
    {
        stats.isSleeping = true;
        float y = GetComponentInChildren<Collider>().bounds.max.y * 1.1f;
        var animalPosition = transformCached.position;
        var p = new Vector3(animalPosition.x, y, animalPosition.z);
        sleepPs = Instantiate(Species.sleepParticleSystem, p, Quaternion.Euler(0f, 0f, 0f), transform);
    }

    public void EndSleep()
    {
        stats.isSleeping = false;
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
        var pos = transformCached.position;

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