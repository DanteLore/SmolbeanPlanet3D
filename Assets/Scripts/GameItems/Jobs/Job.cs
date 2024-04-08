public class Job
{
    public JobSpec JobSpec { get; private set; }
    public SmolbeanBuilding Building { get; private set; }
    public SmolbeanColonist Colonist { get; set; }
    public bool IsTerminated { get; private set; }
    public bool IsOpen { get { return !IsTerminated; } set { IsTerminated = !value; } }

    public Job(SmolbeanBuilding building, JobSpec jobSpec, bool startTerminated = false)
    {
        IsTerminated = startTerminated;
        Building = building;
        JobSpec = jobSpec;
    }

    public void Terminate()
    {
        IsTerminated = true;
    }

    public void Open()
    {
        IsTerminated = false;
    }
}