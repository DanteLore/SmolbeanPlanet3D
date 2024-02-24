using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorterFailedDeliveryRequestState : IState
{
    private PorterDoDeliveryRequestState parent;
    private Porter porter;
    private readonly DeliveryManager deliveryManager;

    public PorterFailedDeliveryRequestState(PorterDoDeliveryRequestState parent, Porter porter, DeliveryManager deliveryManager)
    {
        this.parent = parent;
        this.porter = porter;
        this.deliveryManager = deliveryManager;
    }

    public void OnEnter()
    {
        deliveryManager.ReturnDelivery(porter, porter.DeliveryRequest);
        porter.DeliveryRequest = null;
        parent.SetFinished(true); 
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
