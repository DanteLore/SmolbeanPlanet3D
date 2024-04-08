using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobController : MonoBehaviour, IObjectGenerator
{
    public static JobController Instance { get; private set; }

    public GameObject freeColonistPrefab;

    public JobSpec[] jobSpecs;

    private readonly List<Job> jobs = new();

    public IEnumerable<Job> AllJobs { get { return jobs; } }
    public IEnumerable<Job> Vacancies { get { return jobs.Where(j => j.Colonist == null && !j.IsTerminated); } }
    public IEnumerable<Job> AssignedJobs { get { return jobs.Where(j => j.Colonist != null); } }

    public int Priority { get { return 160; } }
    public bool RunModeOnly { get { return true; } }


    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public Job ClaimNextJob(SmolbeanColonist colonist)
    {
        if (Vacancies.Count() == 0)
            return null;

        var job = Vacancies.ToArray()[UnityEngine.Random.Range(0, Vacancies.Count() - 1)];
        job.Colonist = colonist;
        colonist.Job = job;

        return job;
    }

    public void RegisterJob(JobSpec jobSpec, SmolbeanBuilding building)
    {
        Job job = new(building, jobSpec);
        jobs.Add(job);
    }

    public void Clear()
    {
        jobs.Clear();
    }

    public IEnumerable<Job> GetAllJobsForBuilding(SmolbeanBuilding building)
    {
        return jobs.Where(j => j.Building == building);
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.jobData = jobs.Select(job => new JobSaveData()
        {
            jobSpecIndex = Array.IndexOf(jobSpecs, job.JobSpec),
            isTerminated = job.IsTerminated,
            buildingName = job.Building.name,
            colonistName = job.Colonist != null ? job.Colonist.Stats.name : null
        }).ToList();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.jobData != null)
        {
            jobs.Clear();
            foreach (var row in data.jobData)
            {
                var building = BuildingController.Instance.FindBuildingByName(row.buildingName);
                var colonist = (row.colonistName != null) ? AnimalController.Instance.FindAnimalByNameAndType<SmolbeanColonist>(row.colonistName) : null;
                var jobSpec = jobSpecs[row.jobSpecIndex];

                if (building != null)
                {
                    var job = new Job(building, jobSpec, startTerminated: row.isTerminated) { Colonist = colonist };
                    jobs.Add(job);
                    if(colonist != null)
                        colonist.Job = job;
                }
            }
        }

        return null;
    }

    public void BuildingDestroyed(SmolbeanBuilding building)
    {
        var jobsToTerminate = jobs.Where(v => v.Building == building).ToArray();
        foreach (var job in jobsToTerminate)
        {
            job.Terminate();
            jobs.Remove(job);
        }
    }
}
