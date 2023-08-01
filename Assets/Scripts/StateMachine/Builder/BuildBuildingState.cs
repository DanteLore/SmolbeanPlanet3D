using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBuildingState : IState
{
    private Builder builder;
    private SoundPlayer soundPlayer;

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
    }

    public void OnExit()
    {
        builder.Show();
        soundPlayer.Stop("Working");
        builder.TargetBuilding.EndBuild();
    }

    public void Tick()
    {
        builder.TargetBuilding.DoBuild(Time.deltaTime);
    }
}
