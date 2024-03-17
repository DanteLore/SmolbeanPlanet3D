using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForJobState : IState
{
    private readonly SmolbeanColonist colonist;
    private readonly JobController jobController;

    public SearchForJobState(SmolbeanColonist colonist, JobController jobController)
    {
        this.colonist = colonist;
        this.jobController = jobController;
    }

    public void OnEnter()
    {
        colonist.Job = jobController.ClaimNextJob(colonist);

        if(colonist.Job != null)
            colonist.Think($"Picked up a job doing {colonist.Job.JobSpec.jobName} at {colonist.Job.Building.name}");
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
