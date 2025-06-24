using System;

[Serializable]
public class OfferingPart
{
    public string itemName; 
    public int quantity;

    public OfferingPart(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}
