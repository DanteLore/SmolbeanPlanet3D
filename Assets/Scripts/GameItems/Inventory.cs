using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

public class Inventory
{
    private List<InventoryItem> inventory;

    public Inventory()
    {
        inventory = new List<InventoryItem>();
    }

    public void PickUp(InventoryItem item)
    {
        Assert.IsNotNull(item);
        Assert.IsTrue(item.quantity > 0);

        if(item != null)
            inventory.Add(item);

        MergeStacks();
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
        MergeStacks();
        return inventory.Where(i => i.dropSpec == drop).Sum(i => i.quantity);
    }

    public InventoryItem Take(DropSpec itemSpec, int requiredQuantity)
    {
        Assert.IsTrue(requiredQuantity <= itemSpec.stackSize, "You can't take more than one stack at a time!");

        MergeStacks();

        // Get the largest stack we have
        var item = inventory.Where(i => i.dropSpec == itemSpec).OrderByDescending(i => i.quantity).FirstOrDefault();

        // If it's too small, return null
        if(item == null || item.quantity < requiredQuantity)
            return null;

        // If it's the right size, return it
        if(item.quantity == requiredQuantity)    
            return item;

        // Must be to big, so split it
        item.quantity -= requiredQuantity;
        return new InventoryItem
        {
            dropSpec = item.dropSpec,
            quantity = requiredQuantity
        };
    }

    private void MergeStacks()
    {
        foreach(var items in inventory.GroupBy(i => i.dropSpec, i => i, (k, g) => g.ToList()))
        {
            int i = 0;
            while(i < items.Count)
            {
                if(items[i].quantity < items[i].dropSpec.stackSize)
                {
                    int j = i + 1;
                    while(j < items.Count && items[i].quantity < items[i].dropSpec.stackSize)
                    {
                        int qttyNeededToFill = items[i].dropSpec.stackSize - items[i].quantity;
                        int qttyTaken = Math.Min(qttyNeededToFill, items[j].quantity);
                        items[j].quantity -= qttyTaken;
                        items[i].quantity += qttyTaken;

                        j++;
                    }
                    i = j; 
                }
                else
                {
                    i++; // This stack is full, move to the next
                }
            }
        }

        var emptyItems = inventory.Where(i => i.quantity <= 0).ToList();
        foreach(var emptyItem in emptyItems)
        {
            inventory.Remove(emptyItem);
        }
    }

    public bool Contains(DropSpec itemSpec, int quantity)
    {
        var q = inventory.Where(i => i.dropSpec == itemSpec).Sum(i => i.quantity);

        return q >= quantity;
    }

    public override string ToString()
    {
        return "ItemCount: " + inventory.Count + "\n" + String.Join("\n", inventory.Select(i => i.ToString()));
    }
}
