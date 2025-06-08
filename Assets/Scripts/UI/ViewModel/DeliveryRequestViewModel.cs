using UnityEngine;

[System.Serializable]
public class DeliveryRequestViewModel
{
    DeliveryRequest request;

    public DeliveryRequestViewModel(DeliveryRequest request)
    {
        this.request = request;
    }

    public string BuildingName
    {
        get { return request.Building.name; }
    }

    public Texture2D BuildingThumbnail
    {
        get { return request.Building.IsComplete ? request.Building.BuildingSpec.thumbnail : request.Building.BuildingSpec.siteThumbnail; }
    }

    public string ColonistName
    {
        get
        {
            var colonist = DeliveryManager.Instance.GetAssignedDeliverer(request) as SmolbeanColonist;
            return colonist != null ? colonist.Stats.name : "-";
        }
    }

    public Texture2D ColonistThumbnail
    {
        get
        {
            var colonist = DeliveryManager.Instance.GetAssignedDeliverer(request) as SmolbeanColonist;
            return colonist != null ? colonist.Species.thumbnail : null;
        }
    }

    public string ItemText
    {
        get { return $"{request.Quantity} x {request.Item.dropName}"; }
    }

    public Texture2D ItemThumbnail
    {
        get { return request.Item.thumbnail; }
    }

    public int ItemQuantity
    {
        get { return request.Quantity; }
    }

    public string ItemName
    {
        get { return request.Item.dropName; }
    }

    public int Priority
    {
        get { return request.Priority; }
        set { request.Priority = value; }
    }
}
