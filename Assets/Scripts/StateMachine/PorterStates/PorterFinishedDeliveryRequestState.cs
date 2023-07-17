using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PorterFinishedDeliveryRequestState : IState
{
    private PorterDoDeliveryRequestState parent;

    public PorterFinishedDeliveryRequestState(PorterDoDeliveryRequestState parent)
    {
        this.parent = parent;
    }

    public void OnEnter()
    {
        parent.SetFinished(true);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
