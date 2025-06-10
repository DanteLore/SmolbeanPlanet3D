using UnityEngine.EventSystems;

public abstract class BuffInstance
{
    public BuffSpec Spec { get { return GetBuffSpec(); } }
    public abstract void ApplyTo(AnimalStats stats, float timeDelta);
    protected abstract BuffSpec GetBuffSpec();
}
