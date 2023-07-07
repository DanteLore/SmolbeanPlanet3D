using System;
using UnityEngine;

public class ItemStack : MonoBehaviour
{
    public DropSpec dropSpec;

    public int quantity;

    public float CreateTime { get; private set; }

    void Awake()
    {
        CreateTime = Time.time;
    }
}
