public interface IDamagable
{
    public void TakeDamage(float damage);

    public bool IsDead { get; }
}
