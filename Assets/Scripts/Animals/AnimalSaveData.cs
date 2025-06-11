using System;
using System.Text;

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
    public BuffInstance[] buffs;

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append("Animal Save Data:");
        sb.Append($"Species Index: {speciesIndex}");
        sb.Append($"Prefab Index: {prefabIndex}");

        return sb.ToString();
    }
}
