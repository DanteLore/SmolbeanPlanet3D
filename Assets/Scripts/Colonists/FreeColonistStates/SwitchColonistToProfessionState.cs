public class SwitchColonistToProfessionState : IState
{
    private readonly SmolbeanColonist colonist;

    public SwitchColonistToProfessionState(SmolbeanColonist colonist)
    {
        this.colonist = colonist;
    }

    public void OnEnter()
    {
        if (colonist.Job != null && colonist.Job.IsOpen)
        {
            JobSpec spec = colonist.Job.JobSpec;
            AnimalController.Instance.SwitchAnimal(colonist, spec.colonistPrefab);
        }
        else
        {
            // If the job was terminated or deleted before now, switch back to a clean free colonist!
            AnimalController.Instance.SwitchAnimal(colonist, JobController.Instance.freeColonistPrefab);
        }
    }

    public void OnExit()
    {
    }

    public void Tick()
    {
    }
}
