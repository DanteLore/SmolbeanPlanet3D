using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public abstract class SmolbeanAnimal : MonoBehaviour
{
    protected void AT(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

    public string natureLayer = "Nature";
    public string creatureLayer = "Creatures";
    public float destinationThreshold = 1.0f;

    public float health;
    public float foodLevel;

    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;
    protected StateMachine stateMachine;

    public AnimalSpec Species { get; set; }

    private GameObject body;

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
        UpdateStats();

        stateMachine.Tick();
    }

    protected virtual void InitialiseStats()
    {
        health = Species.initialHealth;
        foodLevel = Species.initialFoodLevel;
    }

    protected virtual void UpdateStats()
    {
        foodLevel = Mathf.Max(0f, foodLevel - Species.foodDigestedPerSecond * Time.deltaTime);

        if (foodLevel <= Species.starvationThreshold)
        {
            float healthDelta = Species.starvationRatePerSecond * Time.deltaTime;
            healthDelta *= 1f - Mathf.InverseLerp(0f, Species.starvationThreshold, foodLevel);
            health = Mathf.Max(0f, health - healthDelta); 
        }
        else 
        {   
            float healthDelta = Species.healthRecoveryPerSecond * Time.deltaTime;
            healthDelta *= Mathf.InverseLerp(Species.starvationThreshold, Species.maxFoodLevel, foodLevel);
            health = Mathf.Min(Species.maxHealth, health + healthDelta);
        }

        if (health <= 0)
            Die(); 
    }

    private void Die()
    {
        DestroyImmediate(gameObject);
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
        Vector3 v1 = new(transform.position.x, 0.0f, transform.position.z);
        Vector3 v2 = new(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    public bool CloseEnoughTo(GameObject target)
    {
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(natureLayer, creatureLayer));
        return found.Any(c => c.gameObject == target);
    }
}