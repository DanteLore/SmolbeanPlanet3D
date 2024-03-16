public class Job
{
    public JobSpec JobSpec { get; private set; }
    public SmolbeanBuilding Building { get; private set; }
    public SmolbeanColonist Colonist { get; set; }
    public bool IsTerminated { get; private set; }

    public Job(SmolbeanBuilding building, JobSpec jobSpec)
    {
        IsTerminated = false;
        Building = building;
        JobSpec = jobSpec;
    }

    public void Terminate()
    {
        IsTerminated = true;
    }
}