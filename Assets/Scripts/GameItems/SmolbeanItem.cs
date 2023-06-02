using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmolbeanItem : MonoBehaviour, IDamagable
{
    [SerializeField] public NatureObjectSaveData saveData;

    public GameObject destroyParticleSystemPrefab;

    public float maxHealth = 100f;
    public float health = 100f;
    public float healPerSecond = 1f;

    public bool IsDead { get { return health <= 0; }}

    void Update()
    {
        if(IsDead)
        {
            Instantiate(destroyParticleSystemPrefab, transform.position, transform.rotation);
            
            Destroy(gameObject);
        }
        else
        {
            health = Mathf.Min(health + healPerSecond * Time.deltaTime, maxHealth);
        }
    }

    public void TakeDamage(float damage)
    {   
        health -= damage;
    }
}
