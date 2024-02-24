using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public abstract class SmolbeanAnimal : MonoBehaviour
{
    public string natureLayer = "Nature";
    public string creatureLayer = "Creatures";
    public float destinationThreshold = 1.0f;
    
    public Vector3 SpawnPoint { get; private set; }

    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;

    public AnimalSpec Species { get; set; }

    private GameObject body;

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        soundPlayer = GetComponent<SoundPlayer>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;
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