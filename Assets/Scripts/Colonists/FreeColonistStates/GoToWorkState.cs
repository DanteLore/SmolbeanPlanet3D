public class GoToWorkState : IState
{
    private readonly FreeColonist colonist;

    public GoToWorkState(FreeColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        colonist.Think($"Heading to work at {colonist.job.Building.name}");
        colonist.target = colonist.job.Building.spawnPoint.transform.position;
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
