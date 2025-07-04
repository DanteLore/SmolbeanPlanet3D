using System;
using System.Text;

[Serializable]
public class Offering
{
    public OfferingPart[] parts;
    public float availableDuration;
    public float completionDuration;
    public float ritualDuration;
    public float reward;
    public float expiryTime;

    public bool IsExpired { get => DayNightCycleController.Instance.PlayedTime >= expiryTime; }

    public bool IsAccepted { get; private set; }
    public bool IsStarted { get; private set; }

    public Offering(float availableDuration, float completionDuration, float ritualDuration, float reward, OfferingPart[] parts)
    {
        this.availableDuration = availableDuration;
        this.completionDuration = completionDuration;
        this.ritualDuration = ritualDuration;
        this.reward = reward;
        this.parts = parts;
        expiryTime = DayNightCycleController.Instance.PlayedTime + availableDuration;
        IsAccepted = false;
        IsStarted = false;
    }

    public void AcceptOffering()
    {
        expiryTime = DayNightCycleController.Instance.PlayedTime + completionDuration;
        IsAccepted = true;
    }

    public void BeginRitual()
    {
        expiryTime = DayNightCycleController.Instance.PlayedTime + ritualDuration;
        IsStarted = true;
    }

    public bool IsCompletedBy(Inventory inventory)
    {
        for (int i = 0; i < parts.Length; i++)
        {
            if (!inventory.Contains(parts[i].itemName, parts[i].quantity))
                return false;
        }
        return true;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Offering of ");
        for (int i = 0; i < parts.Length; i++)
        {
            if (i > 0)
                sb.Append(", ");

            sb.Append(parts[i].quantity);
            sb.Append(" ");
            sb.Append(parts[i].itemName);
        }

        return sb.ToString();
    }
}
