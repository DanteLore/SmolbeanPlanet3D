using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorterLoadDeliveryItemsState : IState
{
    private Porter porter;

    public PorterLoadDeliveryItemsState(Porter porter)
    {
        this.porter = porter;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
        var delivery = porter.DeliveryRequest;

        if(!porter.Inventory.Contains(delivery.Item, delivery.Quantity))
        {
            var storehouse = (Storehouse)porter.Home;
            var item = storehouse.Inventory.TakeAtLeast(delivery.Item, delivery.Minimum, delivery.Quantity);

            if(item != null)
            {
                porter.Inventory.PickUp(item);
            }
        }
    }
}
