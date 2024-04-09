using System;
using System.Collections.Generic;
using Newtonsoft.Json;  

[Serializable]
public class ResourceCollectionBuildingSaveData : BuildingObjectSaveData
{
    public float collectionZoneCenterX;
    public float collectionZoneCenterY;
    public float collectionZoneCenterZ;
}
