using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInventoryState : IState
{
    private ResourceGatherer gatherer;
    private DropController dropController;

    public DropInventoryState(ResourceGatherer gatherer, DropController dropController)
    {
        this.gatherer = gatherer;
        this.dropController = dropController;
    }

    public void OnEnter()
    {
        
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        var item = gatherer.DropFirstInInventory();

        if(item != null)
            DropController.Instance.Drop(item.dropSpec, gatherer.DropPoint, item.quantity);
    }
}
