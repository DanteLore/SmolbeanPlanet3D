using UnityEngine;

public class Sawmill : FactoryBuilding
{
    public ParticleSystem sawingParticleSystem;

    protected override void Start()
    {
        base.Start();
        
        sawingParticleSystem.Stop();
    }

    public override void StartProcessing()
    {
        base.StartProcessing();

        sawingParticleSystem.Play();
    }

    public override DropSpec StopProcessing()
    {
        sawingParticleSystem.Stop();

        return base.StopProcessing();
    }
}
