using UnityEngine;
using UnityEngine.AI;

public abstract class WalkStateBase : IState
{
    private readonly SmolbeanAnimal animal;
    protected NavMeshAgent navAgent;
    protected Animator animator;
    protected SoundPlayer soundPlayer;
    private Vector3 lastPosition;
    private float lastMoved;
    private float originalAnimatorSpeed;
    protected bool navAgentResetEnabled = true;
    protected float destConfirmedAt;
    private Vector3 walkDestination;

    public float StuckTime { get { return Time.time - lastMoved; } }
    public bool IsStuck { get; set; }
    public string Name { get => GetType().Name; }

    public WalkStateBase(SmolbeanAnimal animal, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
    {
        this.animal = animal;
        this.navAgent = navAgent;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
    }

    protected abstract Vector3 GetDestination();

    public virtual void OnEnter()
    {
        StartNavigation();

        lastPosition = animal.transformCached.position;
        lastMoved = Time.time;

        if (animator != null)
        {
            originalAnimatorSpeed = animator.speed;
            animator.speed = originalAnimatorSpeed * animal.Stats.speed / 3f;
            animator.SetBool("IsWalking", true);
        }

        if (soundPlayer != null)
            soundPlayer.Play("Footsteps");
    }

    private void StartNavigation()
    {
        walkDestination = GetDestination();
        navAgent.SetDestination(walkDestination);
        navAgent.isStopped = false;
        destConfirmedAt = Time.time;
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
            soundPlayer.Stop("Footsteps");
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

        // Confirm our destination is still valid every second
        if (time - destConfirmedAt > 1f)
        {
            // This might happen if the destination has moved, for example if a building was rotated
            // Note:  Ignore Y coord, as it doesn't make a difference for navigation
            Vector3 dest = GetDestination();
            if (dest != walkDestination && Vector3.SqrMagnitude(dest - walkDestination) > 0.1f)
            {
                animal.Think("Destination has changed, recalculating path.");
                StartNavigation();
            }
            destConfirmedAt = time;
        }

        // Start walking
        if (animator != null)
        {
            animator.SetBool("IsWalking", true);
            animator.speed = Mathf.InverseLerp(0f, navAgent.speed, navAgent.velocity.magnitude);
        }

        var pos = animal.transformCached.position;

        if (Vector3.SqrMagnitude(lastPosition - pos) > 1f)
        {
            lastMoved = time;
            lastPosition = pos;
            IsStuck = false;
        }

        if(time - lastMoved > 1f && time - destConfirmedAt > 2f && !IsStuck)
        {
            IsStuck = true;

            animal.Think("I think I'm stuck!");
        }
    }
}
