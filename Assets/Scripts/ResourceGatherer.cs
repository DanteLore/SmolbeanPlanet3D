
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

    void Start()
    {
        var hut = GetComponentInParent<ISmolbeanBuilding>();
        spawnPoint = hut.GetSpawnPoint();
        navAgent = GetComponent<NavMeshAgent>();
        body = transform.Find("Body").gameObject;

        StartCoroutine(GathererLoop());
        lastReportedPosition = transform.position;
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

    protected abstract GameObject GetTarget(Vector3 pos);
    protected IEnumerator GathererLoop()
    {
        yield return new WaitForSeconds(1);

        while(true)
        {
            var target = GetTarget(transform.position);

            if (target == null)
                break;
                
            var dest = target.transform.position;
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
            yield return null;

            float stillTime = 0.0f;
            while (!CloseEnoughTo(target))
            {
                stillTime = (navAgent.velocity.magnitude < 0.5f) ? stillTime + Time.deltaTime : 0.0f;

                if(stillTime > 10.0f)
                {
                    blacklist.Add(target);
                    target = null;
                    yield return new WaitForSeconds(1);
                    break;
                }
                else
                {
                    yield return null;
                }
            }

            navAgent.isStopped = true;

            if(target)
            {
                yield return new WaitForSeconds(3);
                Destroy(target);
                yield return new WaitForSeconds(2);
            }
            dest = spawnPoint;
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
            yield return null;

            while (!CloseEnoughTo(dest))
                yield return null;

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
