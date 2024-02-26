using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;
using Random = UnityEngine.Random;
using System.Collections;

public abstract class SmolbeanAnimal : MonoBehaviour
{
    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);
    protected void AT(IState to, Func<bool> condition) => stateMachine.AddAnyTransition(to, condition);

    public string natureLayer = "Nature";
    public string creatureLayer = "Creatures";
    public float destinationThreshold = 1.0f;

    public float age;
    public float health;
    public float foodLevel;

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
        InitialiseStats();
    }

    protected virtual void Update()
    {
        if (!isDead)
        {
            UpdateStats();

            if (health <= 0f)
                return;

            stateMachine.Tick();
        }
    }

    protected virtual void InitialiseStats()
    {
        health = species.initialHealth;
        foodLevel = Random.Range(species.initialFoodLevelMin, species.initialFoodLevelMax);
    }

    protected virtual void UpdateStats()
    {
        age += Time.deltaTime;
        navAgent.speed = species.speed;

        float ageFactor = Mathf.InverseLerp(species.oldAgeSeconds, species.lifespanSeconds, age);

        // Decreasing health due to old age
        health -= species.oldAgeHealthImpactPerSecond * ageFactor * Time.deltaTime;

        // Digest some food
        foodLevel = Mathf.Max(0f, foodLevel - species.foodDigestedPerSecond * Time.deltaTime);

        if (foodLevel <= species.starvationThreshold)
        {
            // Health decrease sue to starvation
            float healthDelta = species.starvationRatePerSecond * Time.deltaTime;
            healthDelta *= 1f - Mathf.InverseLerp(0f, species.starvationThreshold, foodLevel);
            health = Mathf.Max(0f, health - healthDelta); 
        }
        else
        {
            // Health recover with a full stomach
            float healthDelta = species.healthRecoveryPerSecond * ageFactor * Time.deltaTime;
            healthDelta *= Mathf.InverseLerp(species.starvationThreshold, species.maxFoodLevel, foodLevel);
            health = Mathf.Min(species.maxHealth, health + healthDelta);
        }

        if (health <= 0)
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
        float delta = Mathf.Min(amount, species.maxFoodLevel - foodLevel);
        foodLevel += delta;
        return delta;
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
        return foodLevel > species.fullThreshold;
    }

    public virtual bool IsHungry()
    {
        return foodLevel < species.hungryThreshold;
    }
}