using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public abstract class SmolbeanAnimal : MonoBehaviour
{
    public string natureLayer = "Nature";
    public string dropLayer = "Drops";
    public float destinationThreshold = 1.0f;
    
    public Inventory Inventory { get; private set; }
    public Vector3 SpawnPoint { get; private set; }

    protected Animator animator;
    protected NavMeshAgent navAgent;
    protected SoundPlayer soundPlayer;

    private Vector3 lastReportedPosition;
    private GameObject body;

    private SmolbeanBuilding home;
    public SmolbeanBuilding Home
    {
        get
        {
            // Once colonists break free of their home buildings, this will go away!
            if(home == null)
                home = GetComponentInParent<SmolbeanBuilding>();
            
            return home;
        }
    }

    protected virtual void Start()
    {
        animator = GetComponentInChildren<Animator>();
        soundPlayer = GetComponent<SoundPlayer>();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;
        Inventory = new Inventory();
        SpawnPoint = Home.GetSpawnPoint();
    }    

    protected virtual void Update()
    {
        UpdateGroundWear();
    }

    private void UpdateGroundWear()
    {
        if (!body.activeInHierarchy)
            return;

        float threshold = GroundWearManager.Instance.updateThreshold;
        if (Vector3.SqrMagnitude(transform.position - lastReportedPosition) > threshold * threshold)
        {
            lastReportedPosition = transform.position;
            GroundWearManager.Instance.WalkedOn(transform.position);
        }
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
        Vector3 v1 = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 v2 = new Vector3(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    public bool CloseEnoughTo(GameObject target)
    {
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(natureLayer, dropLayer));
        return found.Any(c => c.gameObject == target);
    }
}