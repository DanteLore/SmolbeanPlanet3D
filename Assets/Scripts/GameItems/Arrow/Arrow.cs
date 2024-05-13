using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private float timeBetweenImpactAndArrowDestruction = 5f;
    private bool flying = true;
    private float impactTime;

    void Update()
    {
        if(flying && rigidBody.velocity.sqrMagnitude > 1f)
            transform.rotation = Quaternion.LookRotation(rigidBody.velocity);
        else if(!flying && Time.time - impactTime > timeBetweenImpactAndArrowDestruction)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(flying)
        {
            flying = false;
            Destroy(rigidBody);
            Destroy(hitCollider);
            Debug.Log($"Hit {other.name}");
            impactTime = Time.time;
        }
    }
}
