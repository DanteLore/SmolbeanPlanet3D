using System.Collections.Generic;

public abstract class BuffInstance
{
    public bool isExpired = false;

    public BuffSpec Spec { get { return GetBuffSpec(); } }
    public abstract IEnumerable<BuffInstance> ApplyTo(AnimalStats stats, float timeDelta);
    protected abstract BuffSpec GetBuffSpec();
}
