using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorterFinishedDeliveryRequestState : IState
{
    private PorterDoDeliveryRequestState parent;
    private Porter porter;

    public PorterFinishedDeliveryRequestState(PorterDoDeliveryRequestState parent, Porter porter)
    {
        this.parent = parent;
        this.porter = porter;
    }

    public void OnEnter()
    {
        parent.SetFinished(true);
        porter.DeliveryRequest = null;
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
