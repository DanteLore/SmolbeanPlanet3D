using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class WalkStateBase : IState
{
    protected NavMeshAgent navAgent;
    protected Animator animator;
    protected SoundPlayer soundPlayer;
    private Vector3 lastPosition;
    private float lastMoved;
    private static readonly float gameSpeedNavJumpSize = 0.5f;
    
    public float StuckTime { get { return Time.time - lastMoved; } }

    public WalkStateBase(NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
    {
        this.navAgent = navAgent;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
    }

    protected abstract Vector3 GetDestination();

    public virtual void OnEnter()
    {        
        navAgent.SetDestination(GetDestination());
        navAgent.isStopped = false;

        lastPosition = navAgent.transform.position;
        lastMoved = Time.time;

        animator?.SetBool("IsWalking", true);
        soundPlayer?.Play("Footsteps");
    }

    public virtual void OnExit()
    {
        navAgent.isStopped = true;
        animator?.SetBool("IsWalking", false);
        soundPlayer?.Stop("Footsteps");
    }

    public void Tick()
    {
        if(Vector3.SqrMagnitude(lastPosition - navAgent.transform.position) >= 0.5f)
        {
            lastMoved = Time.time;
            lastPosition = navAgent.transform.position;
        }

        if(Time.time - lastMoved > 1f)
        {
            // Kick the nav agent after small amount of inactivity
            navAgent.isStopped = true;
            navAgent.SetDestination(GetDestination());
            navAgent.isStopped = false;
        }

        if(Time.timeScale > 1.0f)
            CheckSteeringTargetPosition();
    }

    private float distanceSteeringTarget;
    private void CheckSteeringTargetPosition()
    {
        float d = Vector3.Distance(navAgent.transform.position, navAgent.steeringTarget);
        float jumpThreshold = gameSpeedNavJumpSize * Time.timeScale;

        if(d <= jumpThreshold)
        {
            if(distanceSteeringTarget < d)
            {
                navAgent.transform.position = navAgent.steeringTarget;
            }
            else
            {
                distanceSteeringTarget = d;
            }
        }
    }
}
