using System.Collections;
using UnityEngine;

public class SmolbeanTree : SmolbeanItem
{
    public float treeFallTime = 5f;
    public float treeFallAngle = 85f;

    public GameObject choppedParticleSystemPrefab;

    protected override void Dead()
    {
        StartCoroutine(FallDown());
    }

    private IEnumerator FallDown()
    {
        gameObject.isStatic = false;
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = 1000f;
        rb.drag = .1f;
        //rb.AddForce(Vector3.right, ForceMode.Impulse);

        yield return new WaitForEndOfFrame();

        Instantiate(choppedParticleSystemPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));

        var startTime = Time.time;
        while(Vector3.Angle(Vector3.up, transform.up) < treeFallAngle && Time.time - startTime < treeFallTime)
            yield return new WaitForSeconds(0.1f);

        Instantiate(destroyParticleSystemPrefab, transform.position, Quaternion.Euler(0f, 0f, 0f));
        DropItems();
        Destroy(gameObject);
    }
}
