using UnityEngine;
using UnityEngine.AI;

public class SmolbeanDrop : MonoBehaviour
{
    public DropSpec dropSpec;

    public int quantity;

    private float createTime;
    private float lastCheckTime;

    void Awake()
    {
        createTime = Time.time;
        lastCheckTime = createTime;
    }

    private void Update()
    {
        // Check our own validity every second
        if (Time.time - lastCheckTime > 1f)
        {
            // If we fell off the nav mesh, we must die
            float age = Time.time - createTime;

            if (age > dropSpec.lifeSpanSeconds)
                Destroy(gameObject);

            if (age > 5 &&
                !NavMesh.SamplePosition(transform.position, out var _, 0.5f, NavMesh.AllAreas))
                Destroy(gameObject);

            lastCheckTime = Time.time;
        }
    }

    public bool IsFull()
    {
        return quantity >= dropSpec.stackSize;
    }
}
