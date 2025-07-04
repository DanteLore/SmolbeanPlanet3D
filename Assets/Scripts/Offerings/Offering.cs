using System;
using System.Text;

[Serializable]
public class Offering
{
    public OfferingPart[] parts;
    public float availableDuration;
    public float completionDuration;
    public float reward;
    public float expiryTime;

    public bool IsExpired { get { return DayNightCycleController.Instance.PlayedTime >= expiryTime; } }

    public bool IsStarted { get; private set; }

    public Offering(float availableDuration, float completionDuration, float reward, OfferingPart[] parts)
    {
        this.availableDuration = availableDuration;
        this.completionDuration = completionDuration;
        this.reward = reward;
        this.parts = parts;
        expiryTime = DayNightCycleController.Instance.PlayedTime + availableDuration;
        IsStarted = false;
    }

    public void StartOffering()
    {
        expiryTime = DayNightCycleController.Instance.PlayedTime + completionDuration;
        IsStarted = true;
    }

    public void CompleteOffering()
    {
        IsStarted = false;
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
