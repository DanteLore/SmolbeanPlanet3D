public class Job
{
    public JobSpec JobSpec { get; private set; }
    public SmolbeanBuilding Building { get; private set; }

    public Job(SmolbeanBuilding building, JobSpec jobSpec)
    {
        Building = building;
        JobSpec = jobSpec;
    }
}