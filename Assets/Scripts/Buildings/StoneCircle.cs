using UnityEngine;

public class StoneCircle : SmolbeanBuilding
{
    protected override void Start()
    {
        base.Start();

        OfferingController.Instance.OnOfferingCreated += OfferingCreated;
        OfferingController.Instance.OnOfferingCompleted += OfferingCompleted;
        OfferingController.Instance.OnOfferingExpired += OfferingExpired;
    }

    private void OfferingExpired(Offering offering)
    {
        Debug.Log($"STONE CIRCLE: Offering Expired ☠️ {offering}");
    }

    private void OfferingCompleted(Offering offering)
    {
        Debug.Log($"STONE CIRCLE: Offering Completed ✅ {offering}");
    }

    private void OfferingCreated(Offering offering)
    {
        Debug.Log($"STONE CIRCLE: Offering Created 🌟 {offering}");
    }
}
