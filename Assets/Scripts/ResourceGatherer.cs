
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public abstract class ResourceGatherer : MonoBehaviour
{
    public string NatureLayer = "Nature";
    public float destinationThreshold = 1.0f;
    public enum ResourceGathererState {Resting, Walking, Gathering}
    public ResourceGathererState state = ResourceGathererState.Resting;
    protected NavMeshAgent navAgent;
    protected GameObject body;
    protected List<GameObject> blacklist = new List<GameObject>();
    protected Vector3 spawnPoint;
    private Vector3 lastReportedPosition;

    void Start()
    {
        var hut = GetComponentInParent<ISmolbeanBuilding>();
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

    IEnumerator SolveStuck() {
        Vector3 lastPosition = this.transform.position;
 
        while (true) {
            yield return new WaitForSeconds(3f);
 
            //Maybe we can also use agent.velocity.sqrMagnitude == 0f or similar
            if (!navAgent.pathPending && navAgent.hasPath && navAgent.remainingDistance > navAgent.stoppingDistance) {
                Vector3 currentPosition = this.transform.position;
                if (Vector3.Distance(currentPosition, lastPosition) < 1f) 
                {
                    Vector3 destination = navAgent.destination;
                    navAgent.ResetPath();
                    navAgent.SetDestination(destination);
                    Debug.Log("Agent Is Stuck");
                }
                Debug.Log("Current Position " + currentPosition + " Last Position " + lastPosition);
                lastPosition = currentPosition;
            }
        }
    }

    protected abstract GameObject GetTarget(Vector3 pos);
    protected IEnumerator GathererLoop()
    {
        state = ResourceGathererState.Resting;
        yield return new WaitForSeconds(1);

        while(true)
        {
            var target = GetTarget(transform.position);

            if (target == null)
                break;
                
            var dest = target.transform.position;
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
            state = ResourceGathererState.Walking;
            yield return null;

            while (!CloseEnoughTo(target))
            {
                yield return null;
            }

            state = ResourceGathererState.Gathering;
            navAgent.isStopped = true;

            if(target)
            {
                yield return new WaitForSeconds(3);
                Destroy(target);
                yield return new WaitForSeconds(2);
            }

            state = ResourceGathererState.Walking;
            dest = spawnPoint;
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
            yield return null;

            while (!CloseEnoughTo(dest))
                yield return null;

            state = ResourceGathererState.Resting;
            navAgent.isStopped = true;
            body.SetActive(false);
            yield return new WaitForSeconds(5);

            body.SetActive(true);
        }
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
