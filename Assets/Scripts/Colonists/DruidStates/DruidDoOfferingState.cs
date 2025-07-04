using UnityEngine;

public class DruidDoOfferingState : IState
{
    private readonly Druid druid;
    private readonly SoundPlayer soundPlayer;

    protected StoneCircle StoneCircle
    {
        get
        {
            Debug.Assert(druid.Job != null, "Should not get into this state if the colonist has no job!");
            return (StoneCircle)druid.Job.Building;
        }
    }

    public DruidDoOfferingState(Druid druid, SoundPlayer soundPlayer)
    {
        this.druid = druid;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        druid.Hide();
        soundPlayer.Play("Working");

        StoneCircle.StartOffering();
    }

    public void OnExit()
    {
        druid.Show();
        soundPlayer.Stop("Working");

        if (StoneCircle != null)
        {
            StoneCircle.StopOffering();
        }
    }

    public void Tick()
    {
        
    }
}
