public class SwitchColonistToProfessionState : IState
{
    private readonly SmolbeanColonist colonist;

    public SwitchColonistToProfessionState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        JobSpec spec = colonist.job.JobSpec;
        AnimalController.Instance.SwitchAnimal(colonist, spec.colonistPrefab);
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
