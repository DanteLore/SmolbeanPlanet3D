using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private float timeBetweenImpactAndArrowDestruction = 5f;
    private bool flying = true;
    private float impactTime;

    private void Update()
    {
        if(flying && rigidBody.velocity.sqrMagnitude > 1f)
            transform.rotation = Quaternion.LookRotation(rigidBody.velocity);
        else if(!flying && Time.time - impactTime > timeBetweenImpactAndArrowDestruction)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!flying)
            return;

        flying = false;
        Destroy(rigidBody);
        Destroy(hitCollider);

        if(other.transform.parent.TryGetComponent<SmolbeanAnimal>(out var animal))
        {
            animal.TakeDamage(100);
            Destroy(gameObject);
        }
        else
        {
            impactTime = Time.time;
        }
    }
}
