using System.Linq;

public class NumberOfJobsDataCollector : DataCollectionSeries
{
    private JobController jobController;

    private void Start()
    {
        jobController = GetComponent<JobController>();
    }

    protected override float GetDataValue()
    {
        return jobController.AllJobs.Count();
    }
}
