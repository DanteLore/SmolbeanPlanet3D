public class GoToWorkState : IState
{
    private readonly SmolbeanColonist colonist;

    public GoToWorkState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        colonist.Think($"Heading to work at {colonist.Job.Building.name}");
        colonist.target = colonist.Job.Building.spawnPoint.transform.position;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
