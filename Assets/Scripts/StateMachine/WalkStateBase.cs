using UnityEngine;
using UnityEngine.AI;

public abstract class WalkStateBase : IState
{
    protected ResourceGatherer gatherer;
    protected NavMeshAgent navAgent;
    protected Animator animator;
    private Vector3 lastPosition;
    private float lastMoved;
    
    public float StuckTime { get { return Time.time - lastMoved; } }

    public WalkStateBase(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator)
    {
        this.gatherer = gatherer;
        this.navAgent = navAgent;
        this.animator = animator;
    }

    protected abstract Vector3 GetDestination();

    public void OnEnter()
    {        
        navAgent.SetDestination(GetDestination());
        navAgent.isStopped = false;

        lastPosition = gatherer.transform.position;
        lastMoved = Time.time;

        animator.SetBool("IsWalking", true);
    }

    public void OnExit()
    {
        navAgent.isStopped = true;
        animator.SetBool("IsWalking", false);
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
