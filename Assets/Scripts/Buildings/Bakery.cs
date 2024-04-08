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

    protected override void Update()
    {
        base.Update();

        foreach (var ps in smokeParticleSystems)
            ps.transform.rotation = WindController.Instance.WindRotation;
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
