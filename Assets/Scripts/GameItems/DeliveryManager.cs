using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    private List<DeliveryRequest> unclaimedRequests;
    private Dictionary<IDeliverDrops, DeliveryRequest> claimedRequests;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        unclaimedRequests = new List<DeliveryRequest>();
        claimedRequests = new Dictionary<IDeliverDrops, DeliveryRequest>();
    }

    public DeliveryRequest CreateRequest(SmolbeanBuilding building, DropSpec item, int quantity, int priority = 10)
    {
        var request = new DeliveryRequest(building, item, quantity, priority);
        unclaimedRequests.Add(request);

        return request;
    }

    private bool CanFulfilFrom(DeliveryRequest request, Inventory inventory)
    {
        return inventory.Contains(request.Item, request.Quantity);
    }

    public DeliveryRequest ClaimNextRequest(IDeliverDrops porter, Inventory sourceInventory)
    {
        var request = unclaimedRequests.Where(req => CanFulfilFrom(req, sourceInventory)).OrderBy(r => r.Priority).FirstOrDefault();
        if(request != null)
        {
            unclaimedRequests.Remove(request);
            claimedRequests[porter] = request;
        }

        return request;
    }

    public void LogFinished(IDeliverDrops owner, DeliveryRequest request)
    {
        request.SetComplete(true);
        unclaimedRequests.Remove(request);
        claimedRequests.Remove(owner);
    }
}
