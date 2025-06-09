using UnityEngine;

public abstract class BuffSpec : ScriptableObject
{
    public string buffName;
    public string description;
    public Texture2D thumbnail;

    public abstract BuffInstance GetBuff();
}
