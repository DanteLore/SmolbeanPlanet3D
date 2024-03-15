public class Shipwreck : Storehouse
{
    public JobSpec[] jobSpecs;

    protected override void Start()
    {
        base.Start();

        foreach(var jobSpec in jobSpecs)
            JobController.Instance.RegisterJob(jobSpec, this);
    }
}
