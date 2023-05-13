using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Woodcutter : MonoBehaviour
{
    public WoodcuttersHut hut;
    public Vector3 spawnPoint;
    public string TreeLayer = "Nature";
    public float destinationThreshold = 1.0f;
    
    private NavMeshAgent navAgent;
    private GameObject body;
    private List<GameObject> blacklist = new List<GameObject>();

    void Start()
    {
        hut = GetComponentInParent<WoodcuttersHut>();
        spawnPoint = hut.spawnPoint.transform.position;
        navAgent = GetComponent<NavMeshAgent>();
        body = transform.Find("Body").gameObject;

        StartCoroutine(WoodcuttersLoop());
    }

    private IEnumerator WoodcuttersLoop()
    {
        yield return new WaitForSeconds(1);

        while(true)
        {
            var tree = GetTree(transform.position);

            if (tree == null)
                break;
                
            var dest = tree.transform.position;
            navAgent.SetDestination(dest);
            navAgent.isStopped = false;
            yield return null;

            float stillTime = 0.0f;
            while (!CloseEnoughTo(dest))
            {
                stillTime = (navAgent.velocity.magnitude < 0.5f) ? stillTime + Time.deltaTime : 0.0f;

                if(stillTime > 10.0f)
                {
                    blacklist.Add(tree);
                    tree = null;
                    yield return new WaitForSeconds(1);
                    break;
                }
                else
                {
                    yield return null;
                }
            }

            if(tree)
            {
                navAgent.isStopped = true;
                yield return new WaitForSeconds(3);
                Destroy(tree);
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

    private bool CloseEnoughTo(Vector3 dest)
    {
        Vector3 v1 = new Vector3(transform.position.x, 0.0f, transform.position.z);
        Vector3 v2 = new Vector3(dest.x, 0.0f, dest.z);

        return Vector3.SqrMagnitude(v1 - v2) < destinationThreshold;
    }

    private GameObject GetTree(Vector3 pos)
    {
        var candidates = Physics.OverlapSphere(pos, 500, LayerMask.GetMask(TreeLayer));

        return candidates
            .Select(c => c.gameObject)
            .Except(blacklist)
            .Where(go => go.GetComponent<PineTree>() != null)
            .OrderBy(go => Vector3.SqrMagnitude(go.transform.position - pos))
            .FirstOrDefault();
    }
}
