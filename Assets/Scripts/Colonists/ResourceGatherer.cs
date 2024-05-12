using UnityEngine;
using System.Linq;
using System;

public abstract class ResourceGatherer : SmolbeanColonist, IGatherDrops, IReturnDrops
{
    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float hitCooldown = 1f;
    [SerializeField] protected float idleTime = 1f;
    [SerializeField] protected float sleepTime = 2f;
    [SerializeField] protected int maxStacks = 3;
    [SerializeField] protected DropSpec dropSpec;

    public float HitCooldown { get => hitCooldown; }
    public float Damage { get => damage; }
    public DropSpec DropSpec { get => dropSpec; }

    public GameObject ResourceTarget { get; set; }
    public GameObject TargetDrop { get; set; }

    protected bool DropPointFull()
    {
        return Job.Building.DropPointContents().Where(i => i != null && i.dropSpec == dropSpec).Where(s => s.IsFull()).Count() >= maxStacks;
    }

    public override void InitialiseStats(AnimalStats newStats = null)
    {
        stats = newStats;
    }
}
