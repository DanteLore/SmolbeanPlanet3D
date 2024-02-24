using UnityEngine;

public class IdleState : IState
{
    private Animator animator;
    private float startedIdleTime;

    public float TimeIdle
    {
        get
        {
            return Time.time - startedIdleTime;
        }
    }

    public IdleState(Animator animator)
    {
        this.animator = animator;
    }

    public void OnEnter()
    {
        animator?.SetBool("IsIdle", true);
        startedIdleTime = Time.time;
    }

    public void OnExit()
    {
        animator?.SetBool("IsIdle", false);
    }

    public void Tick()
    {
        
    }
}
