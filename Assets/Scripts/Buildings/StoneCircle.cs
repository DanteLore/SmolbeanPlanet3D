using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoneCircle : SmolbeanBuilding
{
    public ParticleSystem ritualParticleSystem;
    public Material magicFloorMaterial;
    public float edgePositionMax = 0.25f;
    public float edgePositionMin = 0.0f;
    public float fadeInDurationSeconds = 4.0f;
    public float fadeOutDurationSeconds = 2.0f;
    public float fadeUpdatesPerSecond = 30f;

    public int ingredientDeliveryPriority = 4;

    public bool IsStarted { get; private set; }

    private Offering selectedOffering;
    private readonly List<DeliveryRequest> deliveryRequests = new();

    public bool IsReadyToStart
    {
        get => !IsStarted && OfferingController.Instance.Offerings.Where(o => o.IsAccepted).Any(o => o.IsCompletedBy(Inventory));
    }

    public bool IsFinished
    {
        get => selectedOffering.IsStarted && selectedOffering.IsExpired;
    }

    protected override void Start()
    {
        base.Start();
        
        ritualParticleSystem.Stop();
        magicFloorMaterial.SetFloat("_EdgePosition", edgePositionMin);

        OfferingController.Instance.OnOfferingCreated += OfferingCreated;
        OfferingController.Instance.OnOfferingCompleted += OfferingCompleted;
        OfferingController.Instance.OnOfferingExpired += OfferingExpired;

        InvokeRepeating(nameof(UpdateDeliveryRequests), 1.0f, 0.5f);
    }

    private void UpdateDeliveryRequests()
    {
        RemoveCompletedRequests();
        RequestIngedients();
    }

    private void RequestIngedients()
    {
        var startedOfferings = OfferingController.Instance.Offerings.Where(o => o.IsAccepted);

        var itemsToOrder = startedOfferings.SelectMany(o => o.parts)
                                    .GroupBy(p => p.itemName)
                                    .Select(x => (name: x.First().itemName, qtty: x.Sum(p => p.quantity)))
                                    .ToDictionary(kv => kv.name, kv => kv.qtty);

        var itemsOrdered = deliveryRequests.GroupBy(dr => dr.Item.dropName)
                                    .Select(x => (name: x.First().Item.dropName, qtty: x.Sum(dr => dr.Quantity)))
                                    .ToDictionary(kv => kv.name, kv => kv.qtty);

        var itemsInInventory = Inventory.Totals
                                    .Select(x => (name: x.dropSpec.dropName, qtty: x.quantity))
                                    .ToDictionary(kv => kv.name, kv => kv.qtty);

        var itemNames = itemsToOrder.Keys;

        foreach (string itemName in itemNames)
        {
            int required = itemsToOrder[itemName];
            int delivered = itemsInInventory.GetValueOrDefault(itemName, 0);
            int ordered = itemsOrdered.GetValueOrDefault(itemName, 0);

            int numberToOrder = required - delivered - ordered;

            if (numberToOrder > 0)
            {
                DropSpec item = DropController.Instance.DropSpecByName(itemName);
                RaiseDeliveryRequests(item, numberToOrder);
            }
        }
    }

    private void RaiseDeliveryRequests(DropSpec item, int toOrder)
    {
        while (toOrder > 0)
        {
            int ammt = Mathf.Min(toOrder, item.stackSize);
            int min = Mathf.Min(toOrder, item.stackSize);
            var dr = DeliveryManager.Instance.CreateDeliveryRequest(this, item, ammt, minimum:min, priority:ingredientDeliveryPriority);
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

    private void OfferingExpired(Offering offering)
    {
        // Do some funky animation/effect here in future!
        Debug.Log($"STONE CIRCLE: Offering Expired ☠️ {offering}");
    }

    private void OfferingCompleted(Offering offering)
    {
        // Do some funky animation/effect here in future!
        Debug.Log($"STONE CIRCLE: Offering Completed ✅ {offering}");
    }

    private void OfferingCreated(Offering offering)
    {
        // Do some funky animation/effect here in future!
        Debug.Log($"STONE CIRCLE: Offering Created 🌟 {offering}");
    }

    public void StartOffering()
    {
        StartCoroutine(nameof(DoStartRitual));
    }
    
    public void StopOffering()
    {
        StartCoroutine(nameof(DoEndRitual));
    }

    private IEnumerator DoStartRitual()
    {
        Debug.Log($"STONE CIRCLE: The ritual has begun ✨");
        // Start the first offering in the list that we have materials to start
        IsStarted = true;
        selectedOffering = OfferingController.Instance.Offerings.Where(o => o.IsAccepted).First(o => o.IsCompletedBy(Inventory));

        // Burn the ingredients
        foreach (var part in selectedOffering.parts)
        {
            var item = DropController.Instance.DropSpecByName(part.itemName);
            Inventory.TakeMany(item, part.quantity);
        }

        selectedOffering.BeginRitual();

        float waitTime = 1.0f / fadeUpdatesPerSecond;
        float step = (edgePositionMax - edgePositionMin) / (fadeInDurationSeconds * fadeUpdatesPerSecond);
        for (float r = edgePositionMin; r < edgePositionMax; r += step)
        {
            magicFloorMaterial.SetFloat("_EdgePosition", r);
            yield return new WaitForSeconds(waitTime);
        }
        ritualParticleSystem.Play();
    }

    private IEnumerator DoEndRitual()
    {
        if (ritualParticleSystem != null)
            ritualParticleSystem.Stop();

        float waitTime = 1.0f / fadeUpdatesPerSecond;
        float step = (edgePositionMax - edgePositionMin) / (fadeOutDurationSeconds * fadeUpdatesPerSecond);
        for (float r = edgePositionMax; r > edgePositionMin; r -= step)
        {
            magicFloorMaterial.SetFloat("_EdgePosition", r);
            yield return new WaitForSeconds(waitTime);
        }
        
        // Reward the mana
        ManaController.Instance.AddMana(selectedOffering.reward);

        // Mark the offering as complete/remove it
        OfferingController.Instance.Complete(selectedOffering);
        IsStarted = false;
        selectedOffering = null;
    }

}
