using UnityEngine;
using UnityEngine.AI;

public abstract class SmolbeanColonist : SmolbeanAnimal
{
    public string dropLayer = "Drops";

    private Vector3 lastReportedPosition;

    public Job job;

    public SmolbeanBuilding Home
    {
        get
        {
            // TODO: Once colonists break free of their home buildings, this will go away!
            return (job != null) ? job.Building : null;
        }
    }

    public Vector3 SpawnPoint
    {
        get
        {
            return (Home != null) ? Home.spawnPoint.transform.position : Vector3.zero;
        }
    }

    public override void AdoptIdentity(SmolbeanAnimal original)
    {
        base.AdoptIdentity(original);

        var originalColonist = (SmolbeanColonist)original;

        if(originalColonist.job != null && !originalColonist.job.IsTerminated)
        {
            job = originalColonist.job;
            job.Colonist = this;
        }
    }

    protected override void Start()
    {
        base.Start();

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
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