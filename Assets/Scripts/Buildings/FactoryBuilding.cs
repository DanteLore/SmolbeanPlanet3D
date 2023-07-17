using System.Linq;
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
            RemoveCompletedRequests();

            RequestIngedients();

            yield return new WaitForSeconds(5f);
        }
    }

    private void RequestIngedients()
    {
        foreach (var ingredient in recipe.ingredients)
        {
            // If we don't have enough of the resource in the inventory
            if(!Inventory.Contains(ingredient.item, ingredient.quantity))
            {
                RaiseRequestIfNoneExists(ingredient);
            }
        }
    }

    private void RaiseRequestIfNoneExists(Recipe.Ingredient ingredient)
    {
        if (!deliveryRequests.TryGetValue(ingredient, out var request) || request.IsComplete)
        {
            deliveryRequests[ingredient] = DeliveryManager.Instance.CreateRequest(this, ingredient.item, ingredient.quantity);
        }
    }

    private void RemoveCompletedRequests()
    {
        var toRemove = deliveryRequests.Where(kv => kv.Value.IsComplete).Select(kv => kv.Key).ToList();
        foreach (var key in toRemove)
            deliveryRequests.Remove(key);
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
