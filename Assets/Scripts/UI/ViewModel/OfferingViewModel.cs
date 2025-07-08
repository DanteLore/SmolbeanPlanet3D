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
    public bool StartButtonEnabled => !offering.IsAccepted;
    public bool IsAccepted { get => offering.IsAccepted; }
    public bool IsStarted { get => offering.IsStarted; }

    public float InitialDuration
    {
        get
        {
            if (offering.IsStarted)
                return offering.ritualDuration;
            else if (offering.IsAccepted)
                return offering.completionDuration;
            else
                return offering.availableDuration;
        }
    }

    public string ActionButtonText
    {
        get
        {
            if (offering.IsStarted)
                return "Started!";
            else if (offering.IsAccepted)
                return "Accepted";
            else
                return "Accept";
        }
    }


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
        offering.AcceptOffering();
    }
}