using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestResourcesInMineState : IState
{
    private Miner miner;
    private Mine mine;
    private SoundPlayer soundPlayer;
    private float lastHit;

    public HarvestResourcesInMineState(Miner miner, SoundPlayer soundPlayer)
    {
        this.miner = miner;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        miner.Think("Mining some minerals...");
        mine = (Mine)miner.Job.Building;
        soundPlayer.Play("Chopping");
    }

    public void OnExit()
    {
        soundPlayer.Stop("Chopping");
        soundPlayer.Play("Chopped");
    }

    public void Tick()
    {
        if(Time.time - lastHit > miner.mineCooldown)
        {
            lastHit = Time.time;
            var item = mine.TryHarvest();
            if(item != null)
                miner.Inventory.PickUp(item);
        }
    }
}
