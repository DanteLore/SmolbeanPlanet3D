using UnityEngine;

public class Mine : SmolbeanBuilding
{
    public DropSpec dropSpec;
    public float dropProbability = 0.2f;
    public float startingTunnelTime = 2f;
    public float tunnelLengthIncrementPerHarvest = 0.05f;
    public float TunnelTime { get; private set; }

    private Animator animator;

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();

        TunnelTime = startingTunnelTime;
    }

    public InventoryItem TryHarvest()
    {
        if(Random.Range(0f, 1f) < dropProbability)
        {
            TunnelTime += tunnelLengthIncrementPerHarvest;
            return DropController.Instance.CreateInventoryItem(dropSpec, dropSpec.dropRate);
        }
        else
        {
            return null;
        }
    }

    public void StartMining()
    {
        animator.SetBool("IsMining", true);
    }

    public void StopMining()
    {
        animator.SetBool("IsMining", false);
    }
}
