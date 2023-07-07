using UnityEngine;
using UnityEngine.AI;

public abstract class WalkStateBase : IState
{
    protected NavMeshAgent navAgent;
    protected Animator animator;
    protected SoundPlayer soundPlayer;
    private Vector3 lastPosition;
    private float lastMoved;
    
    public float StuckTime { get { return Time.time - lastMoved; } }

    public WalkStateBase(NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
    {
        this.navAgent = navAgent;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
    }

    protected abstract Vector3 GetDestination();

    public void OnEnter()
    {        
        navAgent.SetDestination(GetDestination());
        navAgent.isStopped = false;

        lastPosition = navAgent.transform.position;
        lastMoved = Time.time;

        animator.SetBool("IsWalking", true);
        soundPlayer.Play("Footsteps");
    }

    public void OnExit()
    {
        navAgent.isStopped = true;
        animator.SetBool("IsWalking", false);
        soundPlayer.Stop("Footsteps");
    }

    public void Tick()
    {
        if(Vector3.SqrMagnitude(lastPosition - navAgent.transform.position) >= 1f)
        {
            lastMoved = Time.time;
            lastPosition = navAgent.transform.position;
        }

        if(Time.time - lastMoved > 1f)
        {
            // Kick the nav agent after small amount of
            navAgent.isStopped = true;
            navAgent.SetDestination(GetDestination());
            navAgent.isStopped = false;
        }
    }
}
