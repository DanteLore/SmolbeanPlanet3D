using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using System.Collections;

public abstract class SmolbeanAnimal : MonoBehaviour
{
    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    public string natureLayer = "Nature";
    public string creatureLayer = "Creatures";
    public float destinationThreshold = 1.0f;

    protected AnimalStats stats;
    public AnimalStats Stats
    {
        get { return stats; }
    }

    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;

    protected StateMachine stateMachine;
    public int speciesIndex;
    public AnimalSpec species;
    public Vector3 target;
    private GameObject body;
    protected bool isDead = false;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        soundPlayer = GetComponent<SoundPlayer>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;

        stateMachine = new StateMachine(shouldLog: false);
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            UpdateStats();

            if (stats.health <= 0f)
                return;

            stateMachine.Tick();
        }
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
        stats.health -= species.oldAgeHealthImpactPerSecond * ageFactor * Time.deltaTime;

        // Digest some food
        stats.foodLevel = Mathf.Max(0f, stats.foodLevel - species.foodDigestedPerSecond * Time.deltaTime);

        if (stats.foodLevel <= species.starvationThreshold)
        {
            // Health decrease due to starvation
            float healthDelta = species.starvationRatePerSecond * Time.deltaTime;
            healthDelta *= 1f - Mathf.InverseLerp(0f, species.starvationThreshold, stats.foodLevel);
            stats.health = Mathf.Max(0f, stats.health - healthDelta); 
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
            stats = stats
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

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    public bool CloseEnoughTo(GameObject target)
    {
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(natureLayer, creatureLayer));
        return found.Any(c => c.gameObject == target);
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
}