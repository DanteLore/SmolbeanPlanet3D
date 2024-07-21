using System.Collections;
using UnityEngine;

public class SmolbeanTree : SmolbeanItem
{
    public float treeFallTime = 5f;
    public float treeFallAngle = 85f;
    public float recoilAngle = 10f;
    public float recoilFrequency = 2f;

    public GameObject choppedParticleSystemPrefab;

    private SoundPlayer soundPlayer;
    private bool recoiling = false;
    private float recoilStart;
    private Quaternion startRotation;

    void Awake()
    {
        soundPlayer = GetComponent<SoundPlayer>();
    }

    protected override void Dead()
    {
        StartCoroutine(FallDown());

        recoiling = false;
    }

    private IEnumerator FallDown()
    {
        gameObject.isStatic = false;
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 1000f;
        rb.linearDamping = .1f;

        yield return new WaitForEndOfFrame();

        Instantiate(choppedParticleSystemPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));

        var startTime = Time.time;
        while(Vector3.Angle(Vector3.up, transform.up) < treeFallAngle && Time.time - startTime < treeFallTime)
            yield return new WaitForSeconds(0.1f);

        Instantiate(destroyParticleSystemPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));
        soundPlayer.Play("Break");
        DropItems();
        GetComponent<MeshRenderer>().enabled = false;
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        recoiling = true;
        recoilStart = Time.fixedTime;
        startRotation = transform.rotation;
    }

    protected override void Update()
    {
        base.Update();

        if(recoiling)
        {
            float t = (Time.fixedTime - recoilStart);
            float weight = Mathf.Cos(t);
            float r = Mathf.Lerp(-recoilAngle, recoilAngle, Mathf.Sin(t * (recoilFrequency * 2f * Mathf.PI)));

            transform.rotation = startRotation * Quaternion.Euler(r * weight * Time.fixedDeltaTime, 0f, 0f);

            if(weight <= 0)
                recoiling = false;
        }
    }
}
