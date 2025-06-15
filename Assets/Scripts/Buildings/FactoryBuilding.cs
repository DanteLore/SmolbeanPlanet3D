using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class FactoryBuilding : SmolbeanBuilding
{
    public Recipe recipe;
    public bool IsFinished { get { return (Time.time - startTime) * Speed >= recipe.craftingTime; } }
    public bool IsReadyToStart { get; private set; }
    public bool IsOperating { get; private set; }
    public float Speed { get; protected set; } = 1f;

    public int orderMultiplier = 3;
    public int ingredientDeliveryPriority = 8;

    private List<DeliveryRequest> deliveryRequests;
    private float startTime;

    protected override void Start()
    {
        base.Start();
        IsOperating = false;

        deliveryRequests = new List<DeliveryRequest>();

        InvokeRepeating(nameof(UpdateDeliveryRequests), 1.0f, 0.5f);
        
        if (recipe == null)
            Debug.Log("No recipe set for factopry building");
    }

    private void UpdateDeliveryRequests()
    {
        RemoveCompletedRequests();
        RequestIngedients();
    }

    private void RequestIngedients()
    {
        if (recipe == null)
            return;

        foreach (var ingredient in recipe.ingredients)
        {
            // If we don't have enough of the resource in the inventory
            if (!Inventory.Contains(ingredient.item, ingredient.quantity))
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
        IsOperating = true;
        startTime = Time.time;
        IsReadyToStart = false;
    }

    public virtual DropSpec StopProcessing()
    {
        IsOperating = false;
        return IsFinished ? recipe.createdItem : null;
    }
}
