using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactoryBuilding : SmolbeanBuilding
{
    public Recipe recipe;
    public Inventory Inventory { get; private set; }
    public bool IsFinished { get { return (Time.time  - startTime) >= recipe.craftingTime; } }

    private Dictionary<Recipe.Ingredient, DeliveryRequest> deliveryRequests;

    private float startTime;

    protected override void Start()
    {
        base.Start();

        Inventory = new Inventory();
        deliveryRequests = new Dictionary<Recipe.Ingredient, DeliveryRequest>();

        StartCoroutine(UpdateDeliveryRequests());
    }

    private IEnumerator UpdateDeliveryRequests()
    {
        yield return new WaitForSeconds(5f);

        while(true)
        {
            foreach(var key in deliveryRequests.Keys)
            {
                if(deliveryRequests[key].IsComplete)
                    deliveryRequests.Remove(key);
            }

            foreach(var ingredient in recipe.ingredients)
            {
                if(!deliveryRequests.TryGetValue(ingredient, out var request) || request.IsComplete)
                {
                    deliveryRequests[ingredient] = DeliveryManager.Instance.CreateRequest(this, ingredient.item, ingredient.quantity);
                }
            }

            Debug.Log("delivery requests " + deliveryRequests.Count);

            yield return new WaitForSeconds(5f);
        }
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
