
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public abstract class ResourceGatherer : MonoBehaviour
{
    public string NatureLayer = "Nature";
    public float destinationThreshold = 1.0f;
    protected NavMeshAgent navAgent;
    protected GameObject body;
    protected List<GameObject> blacklist = new List<GameObject>();
    protected Vector3 spawnPoint;
    private Vector3 lastReportedPosition;
    private Vector3 lastPosition;
    private Animator animator;

    public enum ResourceGathererState {Resting, Walking, Gathering, Stuck}
    private ResourceGathererState state = ResourceGathererState.Resting;
    protected ResourceGathererState State
    {
        get { return state; }
        set
        {
            state = value;
            if(animator)
            {
                animator.SetBool("IsWalking", (state == ResourceGathererState.Walking));
            }
        }
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        var hut = GetComponentInParent<SmolbeanBuilding>();
        spawnPoint = hut.GetSpawnPoint();
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        body = transform.Find("Body").gameObject;

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
 
            if (State == ResourceGathererState.Walking & !navAgent.pathPending && navAgent.remainingDistance > navAgent.stoppingDistance) 
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
        return GetTargets(pos).FirstOrDefault(g => PathExists(g.transform.position));
    }

    private bool PathExists(Vector3 target)
    {
        var path = new NavMeshPath();
        return navAgent.CalculatePath(transform.position, path);
    }

    protected IEnumerator GathererLoop()
    {
        State = ResourceGathererState.Resting;
        yield return new WaitForSeconds(1);

        while(true)
        {
            var target = GetTarget(transform.position);
            while(target == null)
            {
                Debug.Log("Can't find any targets in my search radius.  Will try again in a sec");
                yield return new WaitForSeconds(1.0f);
                target = GetTarget(transform.position);
            }

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

                if (target)
                {
                    yield return new WaitForSeconds(3);
                    Destroy(target);
                    yield return new WaitForSeconds(2);
                }
            }

            StartWalkingTo(spawnPoint);
            yield return new WaitForSeconds(0.5f);

            while (!CloseEnoughTo(spawnPoint))
            {
                if (State == ResourceGathererState.Stuck)
                    StartWalkingTo(spawnPoint);
                yield return new WaitForSeconds(0.5f);
            }

            State = ResourceGathererState.Resting;
            navAgent.isStopped = true;
            body.SetActive(false);
            yield return new WaitForSeconds(5);

            body.SetActive(true);
        }
    }

    private void StartWalkingTo(Vector3 dest)
    {
        lastPosition = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        navAgent.SetDestination(dest);
        navAgent.isStopped = false;
        State = ResourceGathererState.Walking;
    }

    protected bool CloseEnoughTo(Vector3 dest)
    {
        Vector3 v1 = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 v2 = new Vector3(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    protected bool CloseEnoughTo(GameObject target)
    {
        var found = Physics.OverlapSphere(transform.position, destinationThreshold, LayerMask.GetMask(NatureLayer));
        return found.Any(c => c.gameObject == target);
    }
}
