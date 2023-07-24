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
        Debug.Log("Delivery requested.  I have " + unclaimedRequests.Count + " requests on the unclaimed queue");
        var request = unclaimedRequests.Where(req => CanFulfilFrom(req, sourceInventory)).OrderBy(r => r.Priority).FirstOrDefault();
        if(request != null)
        {
            Debug.Log("Assigning request for " + request.Quantity + " x " + request.Item.dropName);
            unclaimedRequests.Remove(request);
            claimedRequests[porter] = request;
        }
        else
        {
            Debug.Log("Could not find a suitable request");
        }

        return request;
    }

    public void LogFinished(IDeliverDrops owner, DeliveryRequest request)
    {
        request.SetComplete(true);
        unclaimedRequests.Remove(request);
        claimedRequests.Remove(owner);
    }

    public void BuildingDestroyed(SmolbeanBuilding building)
    {
        var unclaimedToRemove = unclaimedRequests.Where(r => r.Building == building).ToList();
        var claimedToRemove = claimedRequests.Where(kv => kv.Value.Building == building).Select(kv => kv.Key).ToList();

        foreach(var request in unclaimedToRemove)
        {
            request.SetComplete(true);
            unclaimedRequests.Remove(request);
        }

        foreach(var owner in claimedToRemove)
        {
            claimedRequests[owner].SetComplete(true);
            claimedRequests.Remove(owner);
        }
    }
}
