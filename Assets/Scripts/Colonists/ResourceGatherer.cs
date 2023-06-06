
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using System;

public abstract class ResourceGatherer : MonoBehaviour
{
    public string natureLayer = "Nature";
    public string dropLayer = "Drops";
    public float destinationThreshold = 1.0f;
    public float damage = 20f;
    public DropSpec dropSpec;
    protected NavMeshAgent navAgent;
    protected GameObject body;
    protected Vector3 spawnPoint;
    protected Vector3 dropPoint;
    private Vector3 lastReportedPosition;
    private Vector3 lastPosition;
    private Animator animator;
    private Stack<InventoryItem> inventory;

    public enum ResourceGathererState {Resting, Walking, Gathering, Stuck}
    private ResourceGathererState state = ResourceGathererState.Resting;
    protected ResourceGathererState State
    {
        get { return state; }
        set 
        { 
            state = value; 
            if(animator != null)
            {
                animator.SetBool("IsIdle", (state == ResourceGathererState.Resting));
                animator.SetBool("IsWalking", (state == ResourceGathererState.Walking));
                animator.SetBool("IsStuck", (state == ResourceGathererState.Stuck));

                if(state == ResourceGathererState.Gathering)
                    animator.SetTrigger(GetGatheringTrigger());
                else
                    animator.ResetTrigger(GetGatheringTrigger());
            }
        }
    }

    protected abstract string GetGatheringTrigger();

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        var hut = GetComponentInParent<SmolbeanBuilding>();
        spawnPoint = hut.GetSpawnPoint();
        dropPoint = hut.GetDropPoint();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;
        inventory = new Stack<InventoryItem>();

        StartCoroutine(GathererLoop());
        lastReportedPosition = transform.position;

        StartCoroutine(SolveStuck());
    }

    void Update()
    {
        if(!body.activeInHierarchy)
            return;

        float threshold = GroundWearManager.Instance.updateThreshold;
        if(Vector3.SqrMagnitude(transform.position - lastReportedPosition) > threshold * threshold)
        {
            lastReportedPosition = transform.position;
            GroundWearManager.Instance.WalkedOn(transform.position);
        }
    }

    IEnumerator SolveStuck() 
    {
        lastPosition = transform.position;
 
        while (true) 
        {
            yield return new WaitForSeconds(10.0f);
 
            if (State == ResourceGathererState.Walking && !navAgent.pathPending && navAgent.remainingDistance > navAgent.stoppingDistance) 
            {
                if (Vector3.Distance(transform.position, lastPosition) <= 1.0f) 
                {
                    //Vector3 destination = navAgent.destination;
                    navAgent.ResetPath();
                    State = ResourceGathererState.Stuck;
                    Debug.Log("Agent Is Stuck");
                }
                lastPosition = transform.position;
            }
        }
    }

    protected abstract IEnumerable<GameObject> GetTargets(Vector3 pos);

    private GameObject GetTarget(Vector3 pos)
    {
        // this just doesn't seem to work - come back to it!
        return GetTargets(pos).FirstOrDefault();  //.FirstOrDefault(g => PathExists(g.transform.position));
    }

    private bool PathExists(Vector3 target)
    {
        NavMesh.SamplePosition(transform.position, out var fromHit, 10f, navAgent.areaMask);
        NavMesh.SamplePosition(target, out var toHit, 10f, navAgent.areaMask);

        var path = new NavMeshPath();
        return NavMesh.CalculatePath(fromHit.position, toHit.position, navAgent.areaMask, path);
    }

    protected IEnumerator GathererLoop()
    {
        while(true)
        {
            State = ResourceGathererState.Resting;
            yield return new WaitForSeconds(1);

            var target = GetTarget(transform.position);

            while(target == null)
            {
                Debug.Log("Can't find any targets in my search radius.  Will try again in a sec");
                yield return new WaitForSeconds(1.0f);
                target = GetTarget(transform.position);
            }

            State = ResourceGathererState.Walking;
            StartWalkingTo(target.transform.position);
            yield return new WaitForSeconds(0.5f);
                
            while (!CloseEnoughTo(target))
            {
                if (State == ResourceGathererState.Stuck || target == null)
                {
                    Debug.Log("Giving up and going home");
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            if (State != ResourceGathererState.Stuck && target != null)
            {
                State = ResourceGathererState.Gathering;
                navAgent.isStopped = true;

                navAgent.updateRotation = false;
                var n = target.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(new Vector3(n.x, 0, n.z));

                var damageable = target.GetComponent<IDamagable>();
                while(!damageable.IsDead)
                {
                    yield return new WaitForSeconds(1f);
                    damageable.TakeDamage(damage);
                    yield return new WaitForEndOfFrame();
                }

                while(target != null)
                {
                    State = ResourceGathererState.Resting;
                    yield return new WaitForSeconds(1);
                }

                target = GetDropTarget();
            }

            while (target == null && !CloseEnoughTo(target))
            {
                if (State == ResourceGathererState.Stuck)
                {
                    Debug.Log("Giving up and going home");
                    break;
                }
                yield return new WaitForSeconds(0.5f);
            }

            if (State != ResourceGathererState.Stuck && target != null)
            {
                var stack = target.GetComponent<ItemStack>();
                var item = DropController.Instance.Pickup(stack);
                inventory.Push(item);
            }

            State = ResourceGathererState.Walking;
            StartWalkingTo(dropPoint);
            yield return new WaitForSeconds(0.1f);

            while (!CloseEnoughTo(dropPoint))
            {
                if (State == ResourceGathererState.Stuck)
                    StartWalkingTo(spawnPoint);
                yield return new WaitForSeconds(0.1f);
            }

            while(inventory.Any())
            {
                var item = inventory.Pop();
                DropController.Instance.Drop(item.dropSpec, dropPoint, item.quantity);
            }

            State = ResourceGathererState.Walking;
            StartWalkingTo(spawnPoint);
            yield return new WaitForSeconds(0.1f);

            while (!CloseEnoughTo(spawnPoint))
            {
                if (State == ResourceGathererState.Stuck)
                    StartWalkingTo(spawnPoint);
                yield return new WaitForSeconds(0.1f);
            }

            State = ResourceGathererState.Resting;
            navAgent.isStopped = true;
            body.SetActive(false);
            yield return new WaitForSeconds(5);

            body.SetActive(true);
        }
    }

    private GameObject GetDropTarget()
    {
        var candidates = Physics.OverlapSphere(transform.position, 20f, LayerMask.GetMask(dropLayer));

        return candidates
            .Select(c => c.gameObject.GetComponent<ItemStack>())
            .Where(i => i != null && i.dropSpec == dropSpec)
            .Select(i => i.gameObject)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - transform.position))
            .FirstOrDefault();
    }

    private void StartWalkingTo(Vector3 dest)
    {
        State = ResourceGathererState.Walking;
        navAgent.updateRotation = true;
        lastPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        navAgent.SetDestination(dest);
        navAgent.isStopped = false;
    }

    protected bool CloseEnoughTo(Vector3 dest)
    {
        Vector3 v1 = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 v2 = new Vector3(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    protected bool CloseEnoughTo(GameObject target)
    {
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(natureLayer));
        return found.Any(c => c.gameObject == target);
    }
}
