using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private float timeBetweenImpactAndArrowDestruction = 5f;
    [SerializeField] private float maxFlightTime = 30f;
    private bool flying = true;
    private float impactTime;
    private float flightStartTime;

    public bool Flying { get { return flying; }}

    private void Start()
    {
        flightStartTime = Time.time;
    }

    private void Update()
    {
        if(flying && rigidBody.linearVelocity.sqrMagnitude > 1f)
            transform.rotation = Quaternion.LookRotation(rigidBody.linearVelocity);
        if(flying && Time.time - flightStartTime > maxFlightTime)
            Destroy(gameObject); // In case we glitched and never hit anything
        if(!flying && Time.time - impactTime > timeBetweenImpactAndArrowDestruction)
            Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!flying)
            return;

        var soundPlayer = GetComponent<SoundPlayer>();
        if(soundPlayer != null)
            soundPlayer.Play("ArrowHit");
        
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
