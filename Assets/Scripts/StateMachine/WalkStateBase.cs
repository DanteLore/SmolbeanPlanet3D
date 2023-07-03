using UnityEngine;
using UnityEngine.AI;

public abstract class WalkStateBase : IState
{
    protected ResourceGatherer gatherer;
    protected NavMeshAgent navAgent;
    protected Animator animator;
    protected SoundPlayer soundPlayer;
    private Vector3 lastPosition;
    private float lastMoved;
    
    public float StuckTime { get { return Time.time - lastMoved; } }

    public WalkStateBase(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
    {
        this.gatherer = gatherer;
        this.navAgent = navAgent;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
    }

    protected abstract Vector3 GetDestination();

    public void OnEnter()
    {        
        navAgent.SetDestination(GetDestination());
        navAgent.isStopped = false;

        lastPosition = gatherer.transform.position;
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
        if(Vector3.SqrMagnitude(lastPosition - gatherer.transform.position) >= 1.0f)
        {
            lastMoved = Time.time;
            lastPosition = gatherer.transform.position;
        }
    }
}
