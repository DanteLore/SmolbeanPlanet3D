using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public void CreateRequest(SmolbeanBuilding building, DropSpec item, int quantity, int priority)
    {
        var request = new DeliveryRequest(building, item, quantity, priority);
        unclaimedRequests.Add(request);
    }

    public DeliveryRequest ClaimNextRequest(IDeliverDrops porter)
    {
        var request = unclaimedRequests.OrderBy(r => r.Priority).FirstOrDefault();
        claimedRequests[porter] = request;
        return request;
    }
}
