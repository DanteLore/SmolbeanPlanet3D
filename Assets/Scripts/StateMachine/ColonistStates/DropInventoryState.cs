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
        var item = colonist.Inventory.DropFirst();

        if(item != null)
            DropController.Instance.Drop(item.dropSpec, colonist.DropPoint, item.quantity);
    }
}
