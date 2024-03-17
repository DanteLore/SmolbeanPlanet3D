using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropInventoryAtDropPointState : IState
{
    private IReturnDrops colonist;
    private DropController dropController;

    public DropInventoryAtDropPointState(IReturnDrops colonist, DropController dropController)
    {
        this.colonist = colonist;
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
        var item = colonist.Inventory.DropLast();

        if (item != null)
        {
            ((SmolbeanColonist)colonist).Think($"Dropping {item.quantity} {item.dropSpec.dropName}");
            dropController.Drop(item.dropSpec, colonist.Job.Building.dropPoint.transform.position, item.quantity);
        }
    }
}
