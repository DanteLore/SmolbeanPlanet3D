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
    private float distanceSteeringTarget;
    private float originalAnimatorSpeed;
    private static readonly float gameSpeedNavJumpSize = 0.5f;
    protected bool navAgentResetEnabled = true;

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

        if (animator != null)
        {
            originalAnimatorSpeed = animator.speed;
            animator.SetBool("IsWalking", true);
        }

        if (soundPlayer != null)
        {
            soundPlayer?.Play("Footsteps");
        }
    }

    public virtual void OnExit()
    {
        if (animator != null)
        {
            animator.speed = originalAnimatorSpeed;
            animator.SetBool("IsWalking", false);
        }

        navAgent.isStopped = true;

        if (soundPlayer != null)
        {
            soundPlayer?.Stop("Footsteps");
        }
    }

    public void Tick()
    {
        if (navAgent.pathPending)
        {
            if (animator != null)
                animator.SetBool("IsWalking", false);

            return;
        }

        if (animator != null)
            animator.SetBool("IsWalking", true);

        var pos = navAgent.transform.position;
        var time = Time.time;

        if(animator != null && navAgent != null)
            animator.speed = Mathf.InverseLerp(0f, navAgent.speed, navAgent.velocity.magnitude);

        if (Vector3.SqrMagnitude(lastPosition - pos) > 0.0f)
        {
            lastMoved = time;
            lastPosition = pos;
        }

        // Commenting this out messes up colonists, but not (seemingly) animals!
        if(navAgentResetEnabled && time - lastMoved > 5f * GameStateManager.Instance.SelectedGameSpeed)
        {
            // Kick the nav agent after small amount of inactivity
            navAgent.isStopped = true;
            navAgent.SetDestination(GetDestination());
            navAgent.isStopped = false;
        }

        if(Time.timeScale > 1.0f)
            CheckSteeringTargetPosition();
    }

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
