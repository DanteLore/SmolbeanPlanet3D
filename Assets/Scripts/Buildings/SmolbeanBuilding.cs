using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public enum BuildingWearPattern { Circle, Rectangle }

public abstract class SmolbeanBuilding : MonoBehaviour
{
    public string dropLayer = "Drops";
    public GameObject spawnPoint;
    public GameObject dropPoint;

    public int PrefabIndex {get; set;}
    public virtual BuildingSpec BuildingSpec {get; set;}
    public BuildingWearPattern wearPattern = BuildingWearPattern.Rectangle;
    public Vector2 wearScale = Vector2.one;
    public GameObject building;
    public float itemDropRadius = 2f;
    public float itemDropHeight = 1f;

    public JobSpec[] jobSpecs;

    public Inventory Inventory { get; private set; }

    public virtual bool IsComplete { get { return true; } }

    public virtual Vector3 GetSpawnPoint()
    {
        return spawnPoint.transform.position;
    }

    public virtual Vector3 GetDropPoint()
    {
        return ((dropPoint != null) ? dropPoint : spawnPoint).transform.position;
    }

    protected virtual void Awake()
    {
        Inventory = new Inventory();
    }

    protected virtual void Start()
    {
        InvokeRepeating(nameof(RegisterWear), 0.0f, 0.5f);

        foreach (var jobSpec in jobSpecs)
            JobController.Instance.RegisterJob(jobSpec, this);
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnDestroy()
    {
        CancelInvoke(nameof(RegisterWear));

        while(!Inventory.IsEmpty() && !GameStateManager.Instance.IsStarted)
        {
            var item = Inventory.DropLast();

            Vector3 upPos = Vector3.up * itemDropHeight;
            Vector3 outPos = Vector3.left * Random.Range(0f, itemDropRadius);
            outPos = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * outPos;

            DropController.Instance.Drop(item.dropSpec, transform.position + upPos + outPos, item.quantity);
        }

        DeliveryManager.Instance.BuildingDestroyed(this);
        JobController.Instance.BuildingDestroyed(this);
    }

    private void RegisterWear()
    {
        GroundWearManager.Instance.BuildingOn(building.GetComponent<Renderer>().bounds, wearPattern, wearScale);
    }

    public List<SmolbeanDrop> DropPointContents(float radius = 2.0f)
    {
        return Physics.OverlapSphere(GetDropPoint(), radius, LayerMask.GetMask(dropLayer))
            .Select(c => c.gameObject.GetComponent<SmolbeanDrop>())
            .ToList();
    }
}
