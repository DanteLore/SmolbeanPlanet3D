public class WoodcuttersHut : SmolbeanBuilding
{
    public JobSpec jobSpec;

    protected override void Start()
    {
        base.Start();

        JobController.Instance.RegisterJob(jobSpec, this);
    }
}