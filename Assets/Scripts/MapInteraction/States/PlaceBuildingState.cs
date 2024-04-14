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
        Vector3 pos = data.SelectedPoint;
        BuildingController.Instance.PlaceBuilding(pos, data.SelectedBuildingSpec);
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
