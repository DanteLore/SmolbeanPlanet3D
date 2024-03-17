using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorkerPickupIngredientsState : IState
{
    private readonly SmolbeanColonist colonist;

    public FactoryWorkerPickupIngredientsState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        if (colonist.Job != null)
        {
            var factory = colonist.Job.Building as FactoryBuilding;
            if (factory != null)
                factory.LoadResources();
        }
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
