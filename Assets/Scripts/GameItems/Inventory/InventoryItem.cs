public class InventoryItem
{
    public DropSpec dropSpec;
    public int quantity;

    public override string ToString()
    {
        return $"{dropSpec.dropName}: {quantity}";
    }
}
