using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDropsState : IState
{
    
    private Porter porter;
    private DropController dropController;

    public StoreDropsState(Porter porter, DropController dropController)
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
            var item = porter.Inventory.DropFirst();
            var storehouse = porter.Home as Storehouse;
            if(storehouse != null)
            {
                storehouse.Inventory.PickUp(item);
            }
        }
    }
}
