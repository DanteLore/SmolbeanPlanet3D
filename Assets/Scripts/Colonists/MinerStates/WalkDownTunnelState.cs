using UnityEngine;

public class WalkDownTunnelState : IState
{
    private Miner miner;
    private Mine mine;
    private SoundPlayer soundPlayer;
    private float startTime;
    public string Name { get => GetType().Name; }

    public float WalkingTime
    {
        get
        {
            return Time.time - startTime;
        }
    }

    public WalkDownTunnelState(Miner miner, SoundPlayer soundPlayer)
    {
        this.miner = miner;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        startTime = Time.time;
        soundPlayer.Play("MuffledFootsteps");
    }

    public void OnExit()
    {
        soundPlayer.Stop("MuffledFootsteps");
    }

    public void Tick()
    {
        
    }
}
