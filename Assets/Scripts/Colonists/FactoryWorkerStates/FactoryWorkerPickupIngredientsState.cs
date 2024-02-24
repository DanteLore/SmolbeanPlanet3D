using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryWorkerPickupIngredientsState : IState
{
    FactoryBuilding factory;

    public FactoryWorkerPickupIngredientsState(FactoryBuilding factory)
    {
        this.factory = factory;
    }

    public void OnEnter()
    {
        factory.LoadResources();
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
