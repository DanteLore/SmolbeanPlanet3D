using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorterStoreDropsState : IState
{
    
    private Porter porter;
    private DropController dropController;

    public PorterStoreDropsState(Porter porter, DropController dropController)
    {
        this.porter = porter;
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
        if(!porter.Inventory.IsEmpty())
        {
            var item = porter.Inventory.DropLast();
            var storehouse = porter.Home as Storehouse;
            if(storehouse != null)
            {
                storehouse.Inventory.PickUp(item);
            }
        }
    }
}
