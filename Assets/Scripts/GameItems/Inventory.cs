using System.Collections.Generic;

public class Inventory
{
    
    private Stack<InventoryItem> inventory;

    public Inventory()
    {
        inventory = new Stack<InventoryItem>();
    }

    public void PickUp(InventoryItem item)
    {
        inventory.Push(item);
    }

    public InventoryItem DropFirst()
    {
        return (inventory.Count > 0) ? inventory.Pop() : null;
    }

    public bool IsEmpty()
    {
        return inventory.Count == 0;
    }
}
