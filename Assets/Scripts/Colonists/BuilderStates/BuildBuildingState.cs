using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBuildingState : IState
{
    private Builder builder;
    private SoundPlayer soundPlayer;
    private float lastUpdateTime;

    public BuildBuildingState(Builder builder, SoundPlayer soundPlayer)
    {
        this.builder = builder;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        builder.Hide();
        soundPlayer.Play("Working");
        builder.TargetBuilding.StartBuild();
        lastUpdateTime = Time.time;
    }

    public void OnExit()
    {
        builder.Show();
        soundPlayer.Stop("Working");
        
        if(builder.TargetBuilding != null)
        {
            soundPlayer.Play("Thud");
            builder.TargetBuilding.EndBuild();
        }
    }  

    public void Tick()
    {
        // Can't use Time.deltaTime as we might not be called every frame
        float t = Time.time;
        float dt = t - lastUpdateTime;
        lastUpdateTime = t;

        builder.TargetBuilding.DoBuild(dt);
    }
}
