using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FindRestingPlaceState
    : IState
{
    private readonly SmolbeanColonist colonist;

    public FindRestingPlaceState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        // TODO: Come back to this when there is somewhere for colonists to rest!
        colonist.target = ShipwreckManager.Instance.Shipwreck.spawnPoint.transform.position;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
