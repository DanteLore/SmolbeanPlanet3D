using UnityEngine;

[System.Serializable]
public class JobViewModel
{
    private Job job;

    public JobViewModel(Job job)
    {
        this.job = job;
    }

    public string JobTitle
    {
        get { return job.JobSpec.jobName; }
    }

    public string ColonistName
    {
        get { return job.Colonist ? job.Colonist.Stats.name : ""; }
    }

    public string BuildingName
    {
        get { return job.Building.name; }
    }

    public Texture2D JobThumbnail
    {
        get { return job.JobSpec.thumbnail; }
    }

    public Texture2D ColonistThumbnail
    {
        get { return job.Colonist ? job.Colonist.Species.thumbnail : null; }
    }

    public Texture2D BuildingThumbnail
    {
        get { return job.Building.BuildingSpec.thumbnail; }
    }

    public bool Enabled
    {
        get
        {
            return job.IsOpen;
        }
        set
        {
            if (value)
                job.Open();
            else
                job.Terminate();
        }
    }
}
