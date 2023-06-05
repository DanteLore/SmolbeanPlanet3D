using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public static DropController Instance { get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public GameObject CreateDrop(DropSpec spec, Vector3 position)
    {
        if(spec == null)
            return null;

        var gameObject = Instantiate(spec.lotsPrefab, position, Quaternion.identity, transform);
        var itemStack = gameObject.GetComponent<ItemStack>();
        itemStack.dropSpec = spec;
        itemStack.quantity = spec.dropRate;

        return gameObject;
    }
}
