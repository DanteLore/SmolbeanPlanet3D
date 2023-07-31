using System;
using System.Collections.Generic;
using Newtonsoft.Json;  

[Serializable]
public class BuildingObjectSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationY;
    public int prefabIndex;
    public IEnumerable<InventoryItemSaveData> inventory;
    public bool complete = true;
}
