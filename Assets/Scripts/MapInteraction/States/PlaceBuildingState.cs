public class PlaceBuildingState : IState
{
    private readonly MapInteractionManager mapInteractionManager;
    private readonly SoundPlayer soundPlayer;

    public bool IsComplete { get { return true; } } // Seems daft, but in future it might take more than a frame!

    public PlaceBuildingState(MapInteractionManager mapInteractionManager, SoundPlayer soundPlayer)
    {
        this.mapInteractionManager = mapInteractionManager;
        this.soundPlayer = soundPlayer;
    }

    public void OnEnter()
    {
        BuildingController.Instance.PlaceBuilding(mapInteractionManager.SelectedPoint, mapInteractionManager.SelectedBuildingSpec);
        soundPlayer.Play("Thud");
        //Instantiate(buildingPlacedParticleSystem, center, Quaternion.identity);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
