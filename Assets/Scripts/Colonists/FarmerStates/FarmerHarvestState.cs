using UnityEngine;

public class FarmerHarvestState : IState
{
    private Farmer farmer;
    private Animator animator;

    public bool NoGrassLeft { get; private set; }

    public FarmerHarvestState(Farmer farmer, Animator animator)
    {
        this.farmer = farmer;
        this.animator = animator;
    }

    public void OnEnter()
    {
        NoGrassLeft = false;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {
        var pos = farmer.transform.position + (farmer.transform.rotation * Vector3.forward * 0.5f);
        var available = GroundWearManager.Instance.GetAvailableGrass(pos, searchRadius: 2f);

        if (available <= 1f)
        {
            NoGrassLeft = true;
        }
        else
        {
            float harvestAmount = Mathf.Min(20f * Time.deltaTime, available);
            farmer.grassHarvested += harvestAmount;
            GroundWearManager.Instance.RegisterHarvest(pos, harvestAmount, wearRadius: 2f);
        }
    }
}
