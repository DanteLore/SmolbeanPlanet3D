using UnityEngine;

public class Bakery : FactoryBuilding
{
    public ParticleSystem[] smokeParticleSystems;

    protected override void Start()
    {
        base.Start();
        foreach(var ps in smokeParticleSystems)
            ps.Stop();
    }

    public override void StartProcessing()
    {
        base.StartProcessing();

        foreach(var ps in smokeParticleSystems)
            ps.Play();
    }

    public override DropSpec StopProcessing()
    {
        foreach(var ps in smokeParticleSystems)
            ps.Stop();

        return base.StopProcessing();
    }
}
