using UnityEngine;
using UnityEngine.AI;

public class Colin : MonoBehaviour
{
    public NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo))
            {
                agent.SetDestination(hitInfo.point);
            }
        }
    }
}
