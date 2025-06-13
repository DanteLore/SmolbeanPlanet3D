
using System;

[Serializable]
public class AnimalStats
{
    public string name;
    public float age;
    public float timeOnIsland = 0;
    public float health;
    public float foodLevel;
    public float scale = 1.0f;
    public float speed;
    public float distanceTravelled;

    // State flags
    [NonSerialized]
    public bool isDead;
    [NonSerialized]
    public bool isSleeping;
}