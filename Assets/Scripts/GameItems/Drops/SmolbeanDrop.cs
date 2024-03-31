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
        var pos = transform.position;
        if (age > 5f &&
            (!NavMesh.SamplePosition(pos, out var hit, 2f, NavMesh.AllAreas) ||
             !OnOrAboveMesh(hit.position, pos)))
            Destroy(gameObject);
    }

    private bool OnOrAboveMesh(Vector3 navPos, Vector3 pos)
    {
        if (navPos.y > pos.y + 1f) // Fallen through mesh!
            return false;

        var p1 = new Vector3(navPos.x, 0, navPos.z);
        var p2 = new Vector3(pos.x, 0, pos.z);

        return Vector3.Distance(p1, p2) < 0.1f;
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
