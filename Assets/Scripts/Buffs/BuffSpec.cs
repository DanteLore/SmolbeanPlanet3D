using UnityEngine;

public abstract class BuffSpec : ScriptableObject
{
    public string buffName;
    public string description;
    public string symbol;

    public abstract BuffInstance GetBuff();
}
