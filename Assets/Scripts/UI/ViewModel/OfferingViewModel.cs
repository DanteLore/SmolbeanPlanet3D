using System;
using System.Collections.Generic;

[Serializable]
public class OfferingViewModel
{
    private readonly Offering offering;
    private readonly List<OfferingItemViewModel> items = new();

    public IEnumerable<OfferingItemViewModel> Items { get => items; }

    public float Reward => offering.reward;
    public string RewardString => $"{offering.reward:0.0}";
    public float RemainingTime => offering.expiryTime - DayNightCycleController.Instance.PlayedTime;
    public string RemainingTimeString => DayNightCycleController.Instance.DurationToString(Math.Max(RemainingTime, 0.0f));
    public float InitialDuration => offering.IsStarted ? offering.completionDuration : offering.availableDuration;
    public bool ShowStartButton => !offering.IsStarted;

    public OfferingViewModel(Offering offering)
    {
        this.offering = offering;

        foreach (var item in offering.parts)
        {
            var vm = new OfferingItemViewModel(item.quantity, item.itemName);
            items.Add(vm);
        }
    }

    public void StartOffering()
    {
        offering.StartOffering();
    }
}