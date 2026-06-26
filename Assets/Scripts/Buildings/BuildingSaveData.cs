using System;
using Newtonsoft.Json;

[Serializable]
public class BuildingObjectSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationY;
    public int prefabIndex;
    public InventoryItemSaveData[] inventory;
    public bool complete = true;
}
