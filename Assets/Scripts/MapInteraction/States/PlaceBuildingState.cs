using UnityEngine;

public class PlaceBuildingState : IState
{
    private readonly MapInteractionData data;
    private readonly SoundPlayer soundPlayer;
    private readonly GameObject buildingPlacedParticleSystem;

    public bool IsComplete { get { return true; } } // Seems daft, but in future it might take more than a frame!

    public PlaceBuildingState(MapInteractionData data, SoundPlayer soundPlayer, GameObject buildingPlacedParticleSystem)
    {
        this.data = data;
        this.soundPlayer = soundPlayer;
        this.buildingPlacedParticleSystem = buildingPlacedParticleSystem;
    }

    public void OnEnter()
    {
        var cameraController = Object.FindFirstObjectByType<CameraController>(); // Probably a better way to find this ;)
        float rotationY = cameraController.CameraRotationY;
        rotationY += 180f; // Opposite
        rotationY = Mathf.Round(rotationY / 90f) * 90f; // Round to nearest 90deg

        Vector3 pos = data.SelectedPoint;
        BuildingController.Instance.PlaceBuilding(pos, rotationY, data.SelectedBuildingSpec);
        soundPlayer.Play("Thud");
        Object.Instantiate(buildingPlacedParticleSystem, pos, Quaternion.identity);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
