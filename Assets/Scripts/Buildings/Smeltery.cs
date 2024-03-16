using System;
using System.Collections;
using UnityEngine;

public class Smeltery : FactoryBuilding
{
    public GameObject fireObject;
    public ParticleSystem smokeParticleSystem;

    protected override void Start()
    {
        base.Start();
        fireObject.SetActive(false);
        smokeParticleSystem.Stop();
    }

    public override void StartProcessing()
    {
        base.StartProcessing();

        fireObject.SetActive(true);
        smokeParticleSystem.Play();
    }

    public override DropSpec StopProcessing()
    {
        fireObject.SetActive(false);
        smokeParticleSystem.Stop();

        return base.StopProcessing();
    }
}
