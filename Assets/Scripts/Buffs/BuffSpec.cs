using System;
using UnityEngine;

public abstract class BuffSpec : ScriptableObject
{
    [Serializable]
    public class BuffThoughtConfigRow
    {
        public float probabilitySeconds;
        public string thought;
    }

    public string buffName;
    public string description;
    public string symbol;
    public BuffThoughtConfigRow[] thoughts;

    public abstract BuffInstance GetBuff();
}
