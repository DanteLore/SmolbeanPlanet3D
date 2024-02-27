using UnityEngine;

public class SmolbeanItem : MonoBehaviour, IDamagable
{
    [SerializeField] public NatureObjectSaveData saveData;

    public GameObject destroyParticleSystemPrefab;

    public DropSpec dropSpec;

    public float maxHealth = 100f;
    public float health = 100f;
    public float healPerSecond = 1f;

    public bool IsDead { get { return health <= 0; }}
    private bool deadCalled = false;

    protected virtual void Update()
    {
        if(IsDead && !deadCalled)
        {
            deadCalled = true;
            Dead();
        }
        else if(health < maxHealth)
        {
            health = Mathf.Min(health + healPerSecond * Time.deltaTime, maxHealth);
        }
    }

    protected virtual void Dead()
    {
        Instantiate(destroyParticleSystemPrefab, transform.position, transform.rotation);
        DropItems();
        Destroy(gameObject);
    }

    protected virtual void DropItems()
    {
        DropController.Instance.Drop(dropSpec, transform.position);
    }

    public virtual void TakeDamage(float damage)
    {   
        health -= damage;
    }
}
