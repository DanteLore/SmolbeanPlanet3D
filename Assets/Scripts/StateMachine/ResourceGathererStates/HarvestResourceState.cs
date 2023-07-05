using UnityEngine;
using UnityEngine.AI;

public class HarvestResource : IState
{
    private ResourceGatherer gatherer;
    private NavMeshAgent navAgent;
    private Animator animator;
    private IDamagable target;
    private SoundPlayer soundPlayer;
    private float lastHit = 0;

    public HarvestResource(ResourceGatherer gatherer, NavMeshAgent navAgent, Animator animator, SoundPlayer soundPlayer)
    {
        this.gatherer = gatherer;
        this.navAgent = navAgent;
        this.animator = animator;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        target = gatherer.Target.GetComponent<IDamagable>();
        animator.SetTrigger(gatherer.GatheringTrigger);

        navAgent.updateRotation = false;
        var n = gatherer.Target.transform.position - gatherer.transform.position;
        gatherer.transform.rotation = Quaternion.LookRotation(new Vector3(n.x, 0, n.z));

        soundPlayer.Play("Chopping");
    }

    public void OnExit()
    {
        soundPlayer.Stop("Chopping");
        soundPlayer.Play("Chopped");
        soundPlayer.Play("Falling");

        animator.ResetTrigger(gatherer.GatheringTrigger);

        navAgent.updateRotation = true;
    }

    public void Tick()
    {
        if(Time.time - lastHit > gatherer.hitCooldown)
        {
            lastHit = Time.time;
            target.TakeDamage(gatherer.damage);
        }
    }
}
