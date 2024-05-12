using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public abstract class SmolbeanColonist : SmolbeanAnimal
{
    private Vector3 lastReportedPosition;

    public Job Job { get; set; }
    public SmolbeanHome Home { get; set; }

    public delegate void ColonistSwapEvent(SmolbeanColonist orignal, SmolbeanColonist replacement);
    public ColonistSwapEvent IdentitySwapped;

    public override void AdoptIdentity(SmolbeanAnimal original)
    {
        base.AdoptIdentity(original);

        var originalColonist = (SmolbeanColonist)original;

        if(originalColonist.Job != null && !originalColonist.Job.IsTerminated)
        {
            Job = originalColonist.Job;
            Job.Colonist = this;
        }
        
        Home = originalColonist.Home;
        Home.SwapColonist(originalColonist, this);

        originalColonist.IdentitySwapped?.Invoke(originalColonist, this);
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

    public override AnimalSaveData GetSaveData()
    {
        return new ColonistSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            rotationY = transform.rotation.eulerAngles.y,
            speciesIndex = SpeciesIndex,
            prefabIndex = PrefabIndex,
            stats = stats,
            thoughts = Thoughts.ToArray(),
            homeName = Home.BuildingName,
        };
    }

    public override void LoadFrom(AnimalSaveData saveData)
    {
        base.LoadFrom(saveData);

        if(saveData is ColonistSaveData colonistData)
        {
            // If we're being created by the AnimalController, it won't know what a colonist is, so the Home will be set later
            // Not very neat, but I'm hanging onto the hope that colonists can continue to be treated as animals :S
            string name = colonistData.homeName;
            Home = BuildingController.Instance.FindBuildingByName(name).GetComponent<SmolbeanHome>();
            Home.AddColonist(this);
        }
    }
}