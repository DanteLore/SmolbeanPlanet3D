using UnityEngine;
using UnityEngine.AI;

public abstract class SmolbeanColonist : SmolbeanAnimal
{
    private Vector3 lastReportedPosition;

    public Job Job { get; set; }

    public override void AdoptIdentity(SmolbeanAnimal original)
    {
        base.AdoptIdentity(original);

        var originalColonist = (SmolbeanColonist)original;

        if(originalColonist.Job != null && !originalColonist.Job.IsTerminated)
        {
            Job = originalColonist.Job;
            Job.Colonist = this;
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