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
    private float originalAnimatorSpeed;
    protected bool navAgentResetEnabled = true;
    protected float destSetAt;
      
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
        destSetAt = Time.time;

        navAgent.avoidancePriority = UnityEngine.Random.Range(50, 99);

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
        var time = Time.time;

        // Not finished planning our route yet...
        if (navAgent.pathPending && navAgent.velocity.sqrMagnitude < 0.1f)
        {
            if (animator != null)
                animator.SetBool("IsWalking", false);

            return;
        }

        if(time - destSetAt > 1f)
        {
            // This might happen if the destination has moved, for example if a building was rotated
            Vector3 dest = GetDestination();
            if (navAgent.destination != dest)
                navAgent.SetDestination(dest);
            destSetAt = time;
        }

        // Start walking
        if (animator != null)
            animator.SetBool("IsWalking", true);

        var pos = navAgent.transform.position;

        if(animator != null && navAgent != null)
            animator.speed = Mathf.InverseLerp(0f, navAgent.speed, navAgent.velocity.magnitude);

        if (Vector3.SqrMagnitude(lastPosition - pos) > 0.0f)
        {
            lastMoved = time;
            lastPosition = pos;
        }
    }
}
