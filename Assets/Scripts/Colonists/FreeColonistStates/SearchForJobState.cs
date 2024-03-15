using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForJobState : IState
{
    private readonly FreeColonist colonist;
    private readonly JobController jobController;

    public SearchForJobState(FreeColonist colonist, JobController jobController)
    {
        this.colonist = colonist;
        this.jobController = jobController;
    }

    public void OnEnter()
    {
        colonist.job = jobController.ClaimNextJob(colonist);

        if(colonist.job != null)
            colonist.Think($"Picked up a job doing {colonist.job.JobSpec.jobName} at {colonist.job.Building.name}");
        else
            colonist.Think("Looking for work, but no work here to be found");
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
