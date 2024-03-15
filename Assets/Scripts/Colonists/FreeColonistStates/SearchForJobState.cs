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
        colonist.Think("Looking for a job");
        colonist.job = jobController.FindJob();
    }

    public void OnExit()
    {

    }

    public void Tick()
    {

    }
}
