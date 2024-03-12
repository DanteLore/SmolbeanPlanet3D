using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SmolbeanDrop : MonoBehaviour
{
    public DropSpec dropSpec;

    public int quantity;

    protected float createTime;
    private float lastCheckTime;

    void Awake()
    {
        createTime = Time.time;
        lastCheckTime = createTime;
    }

    private void Update()
    {
        // Check our own validity every second
        if (Time.time - lastCheckTime > 1f)
        {
            PerSecondUpdate(Time.time - createTime);
            lastCheckTime = Time.time;
        }
    }

    protected virtual void PerSecondUpdate(float age)
    {
        // If we're too old, we must die
        if (age > dropSpec.lifeSpanSeconds)
            Destroy(gameObject);

        // If we fell off the nav mesh, we must die
        if (age > 5 &&
            !NavMesh.SamplePosition(transform.position, out var _, 0.5f, NavMesh.AllAreas))
            Destroy(gameObject);
    }

    public bool IsFull()
    {
        return quantity >= dropSpec.stackSize;
    }

    public virtual DropItemSaveData GetSaveData()
    {
        return new DropItemSaveData
        {
            positionX = transform.position.x,
            positionY = transform.position.y,
            positionZ = transform.position.z,
            dropSpecName = dropSpec.dropName,
            quantity = quantity
        };
    }

    public virtual void LoadExtraData(DropItemSaveData saveData)
    {
        // Nothing to do here
    }
}
