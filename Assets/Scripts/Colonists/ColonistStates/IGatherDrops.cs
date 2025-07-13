using UnityEngine;

public interface IGatherDrops
{
    public GameObject TargetDrop { get; set; }

    public Inventory Inventory { get; }
}
