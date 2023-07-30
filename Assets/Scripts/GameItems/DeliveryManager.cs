using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class DeliveryManager : MonoBehaviour
{
    public static DeliveryManager Instance { get; private set; }
    private List<DeliveryRequest> unclaimedDeliveryRequests;
    private Dictionary<IDeliverDrops, DeliveryRequest> claimedDeliveryRequests;
    private List<CollectionRequest> collections;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        unclaimedDeliveryRequests = new List<DeliveryRequest>();
        claimedDeliveryRequests = new Dictionary<IDeliverDrops, DeliveryRequest>();
        collections = new List<CollectionRequest>();
    }

    public DeliveryRequest CreateDeliveryRequest(SmolbeanBuilding building, DropSpec item, int quantity, int priority = 10)
    {
        var request = new DeliveryRequest(building, item, quantity, priority);
        unclaimedDeliveryRequests.Add(request);

        return request;
    }

    private bool CanFulfilDeliveryRequestFrom(DeliveryRequest request, Inventory inventory)
    {
        return inventory.Contains(request.Item, request.Quantity);
    }

    public DeliveryRequest ClaimNextDeliveryRequest(IDeliverDrops porter, Inventory sourceInventory)
    {
        var request = unclaimedDeliveryRequests.Where(req => CanFulfilDeliveryRequestFrom(req, sourceInventory)).OrderBy(r => r.Priority).FirstOrDefault();
        if(request != null)
        {
            unclaimedDeliveryRequests.Remove(request);
            claimedDeliveryRequests[porter] = request;
        }

        return request;
    }

    public void CompleteDelivery(IDeliverDrops owner, DeliveryRequest request)
    {
        request.SetComplete(true);
        unclaimedDeliveryRequests.Remove(request);
        claimedDeliveryRequests.Remove(owner);
    }

    public void BuildingDestroyed(SmolbeanBuilding building)
    {
        var unclaimedToRemove = unclaimedDeliveryRequests.Where(r => r.Building == building).ToList();
        var claimedToRemove = claimedDeliveryRequests.Where(kv => kv.Value.Building == building).Select(kv => kv.Key).ToList();

        foreach(var request in unclaimedToRemove)
        {
            request.SetComplete(true);
            unclaimedDeliveryRequests.Remove(request);
        }

        foreach(var owner in claimedToRemove)
        {
            claimedDeliveryRequests[owner].SetComplete(true);
            claimedDeliveryRequests.Remove(owner);
        }
    }

    public bool IsCollectionClaimed(ItemStack itemStack)
    {
        return collections.Any(c => c.item == itemStack);
    }

    public void ClaimCollection(ItemStack itemStack, IDeliverDrops porter)
    {
        collections.Add(new CollectionRequest { item = itemStack, porter = porter });
    }

    public void CompleteCollectionJob(IDeliverDrops porter)
    {
        collections.RemoveAll(c => c.porter == porter);
    }
}
