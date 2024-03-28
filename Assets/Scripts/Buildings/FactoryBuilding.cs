using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactoryBuilding : SmolbeanBuilding
{
    public Recipe recipe;
    public bool IsFinished { get { return (Time.time  - startTime) >= recipe.craftingTime; } }
    public bool IsReadyToStart { get; private set; }

    public int orderMultiplier = 3;
    public int ingredientDeliveryPriority = 8;

    private List<DeliveryRequest> deliveryRequests;
    private float startTime;

    protected override void Start()
    {
        base.Start();

        deliveryRequests = new List<DeliveryRequest>();

        InvokeRepeating("UpdateDeliveryRequests", 1.0f, 0.5f);
    }

    private void UpdateDeliveryRequests()
    {
        RemoveCompletedRequests();
        RequestIngedients();
    }

    private void RequestIngedients()
    {
        foreach (var ingredient in recipe.ingredients)
        {
            // If we don't have enough of the resource in the inventory
            if(!Inventory.Contains(ingredient.item, ingredient.quantity))
            {
                OrderMaterials(ingredient);
            }
        }
    }

    private void OrderMaterials(Ingredient ingredient)
    {
        if (deliveryRequests.Count(dr => !dr.IsComplete && dr.Item == ingredient.item) == 0)
        {
            RaiseDeliveryRequests(ingredient);
        }
    }

    private void RaiseDeliveryRequests(Ingredient ingredient)
    {
        int toOrder = ingredient.quantity * orderMultiplier;
        while (toOrder > 0)
        {
            int ammt = Mathf.Min(toOrder, ingredient.item.stackSize);
            int min = Mathf.Min(ingredient.quantity, ingredient.item.stackSize);
            var dr = DeliveryManager.Instance.CreateDeliveryRequest(this, ingredient.item, ammt, minimum:min, priority:ingredientDeliveryPriority);
            deliveryRequests.Add(dr);
            toOrder -= ammt;
        }
    }

    private void RemoveCompletedRequests()
    {
        var toRemove = deliveryRequests.Where(dr => dr.IsComplete).ToList();
        foreach (var dr in toRemove)
            deliveryRequests.Remove(dr);
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

        IsReadyToStart = true;

        return true;
    }

    public virtual void StartProcessing()
    {
        startTime = Time.time;
        IsReadyToStart = false;
    }

    public virtual DropSpec StopProcessing()
    {
        return IsFinished ? recipe.createdItem : null;
    }
}
