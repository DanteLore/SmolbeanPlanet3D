using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    
    private List<InventoryItem> inventory;

    public Inventory()
    {
        inventory = new List<InventoryItem>();
    }

    public void PickUp(InventoryItem item)
    {
        inventory.Add(item);
    }

    public InventoryItem DropLast()
    {        
        var item = inventory.LastOrDefault();
        if(item != null)
            inventory.Remove(item);
        return item;
    }

    public bool IsEmpty()
    {
        return inventory.Count == 0;
    }

    public int ItemCount(DropSpec drop)
    {
        return inventory.Where(i => i.dropSpec == drop).Sum(i => i.quantity);
    }

    public InventoryItem Take(DropSpec itemSpec, int quantity)
    {
        var item = inventory.FirstOrDefault(i => i.dropSpec == itemSpec);

        if(item != null)
        {
            item.quantity -= quantity;
            if(item.quantity <= 0)
                inventory.Remove(item);
        }

        return item;
    }

    public bool Contains(DropSpec itemSpec, int quantity)
    {
        var q = inventory.Where(i => i.dropSpec == itemSpec).Sum(i => i.quantity);

        return q >= quantity;
    }
}
