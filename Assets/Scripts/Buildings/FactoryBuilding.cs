using System;
using UnityEngine;

public abstract class FactoryBuilding : SmolbeanBuilding
{
    public Recipe recipe;
    public Inventory Inventory { get; private set; }
    public bool IsFinished { get { return (Time.time  - startTime) >= recipe.craftingTime; } }

    private float startTime;

    protected override void Start()
    {
        base.Start();

        Inventory = new Inventory();
    }

    public bool HasResources()
    {
        foreach(var ingredient in recipe.ingredients)
        {
            if(Inventory.ItemCount(ingredient.item) < ingredient.quantity)
                return false;
        }

        return true;
    }

    public bool LoadResources()
    {
        if(!HasResources())
            return false;

        foreach(var ingredient in recipe.ingredients)
        {
            Inventory.Take(ingredient.item, ingredient.quantity);
        }

        return true;
    }

    public virtual void StartProcessing()
    {
        startTime = Time.time;
    }

    public virtual DropSpec StopProcessing()
    {
        return IsFinished ? recipe.createdItem : null;
    }
}
