using UnityEngine;

public class OperateMineState : IState
{
    private Animator animator;
    private float startTime;
    private Miner miner;

    public float TimeMining
    {
        get
        {
            return Time.time - startTime;
        }
    }

    public OperateMineState(Miner miner, Animator animator)
    {
        this.miner = miner;
        this.animator = animator;
    }

    public void OnEnter()
    {
        startTime = Time.time;
        miner.Hide();
    }

    public void OnExit()
    {
        miner.Show();
    }

    public void Tick()
    {
        
    }
}
