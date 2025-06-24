using UnityEngine;

public class OfferingItemViewModel
{
    private readonly int quantity;
    private readonly string itemName;

    public string DisplayLabel { get => $"{quantity} x {itemName}"; }

    public Texture2D Thumbnail
    {
        get
        {
            DropSpec spec = OfferingController.Instance.GetItemSpecFor(itemName);
            return spec != null ? spec.thumbnail : null;
        }
    }

    public OfferingItemViewModel(int quantity, string itemName)
    {
        this.quantity = quantity;
        this.itemName = itemName;
    }
}