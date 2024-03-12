using UnityEngine;
using UnityEngine.AI;

public abstract class SmolbeanColonist : SmolbeanAnimal
{
    public string dropLayer = "Drops";
    
    public Inventory Inventory { get; private set; }
    public Vector3 SpawnPoint { get; private set; }

    private Vector3 lastReportedPosition;

    private SmolbeanBuilding home;
    public SmolbeanBuilding Home
    {
        get
        {
            // TODO: Once colonists break free of their home buildings, this will go away!
            if(home == null)
                home = GetComponentInParent<SmolbeanBuilding>();
            
            return home;
        }
    }

    protected override void Start()
    {
        base.Start();

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        Inventory = new Inventory();
    }    

    protected override void Update()
    {
        base.Update();

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
}