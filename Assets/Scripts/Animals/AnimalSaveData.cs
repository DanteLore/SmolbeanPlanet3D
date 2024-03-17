using System;

[Serializable]
public class AnimalSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float rotationY;
    public int speciesIndex;
    public AnimalStats stats;
    public int prefabIndex;
    public Thought[] thoughts;
}
