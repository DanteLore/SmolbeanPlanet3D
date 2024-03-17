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

    private readonly List<Job> vacancies = new();
    private readonly List<Job> assignedJobs = new();

    public IEnumerable<Job> Vacancies { get { return vacancies; } }
    public IEnumerable<Job> AssignedJobs { get { return assignedJobs; } }

    public int Priority { get { return 150; } }
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
        if (vacancies.Count == 0)
            return null;

        // TODO:  Random for now.  Need to assign, like delivery requests
        var job = vacancies[UnityEngine.Random.Range(0, vacancies.Count - 1)];
        vacancies.Remove(job);
        assignedJobs.Add(job);
        job.Colonist = colonist;

        return job;
    }

    public void RegisterJob(JobSpec jobSpec, SmolbeanBuilding building)
    {
        if (
            vacancies.Any(job => job.JobSpec == jobSpec && job.Building == building) ||
            assignedJobs.Any(job => job.JobSpec == jobSpec && job.Building == building)
            )
            return; // Job already exists

        Job job = new(building, jobSpec);
        vacancies.Add(job);
    }

    public void Clear()
    {
        vacancies.Clear();
        assignedJobs.Clear();
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.vacancyData = vacancies.Select(job => new JobSaveData()
        {
            jobSpecIndex = Array.IndexOf(jobSpecs, job.JobSpec),
            buildingName = job.Building.name
        }).ToList();

        saveData.assignedJobData = assignedJobs.Select(job => new JobSaveData()
        {
            jobSpecIndex = Array.IndexOf(jobSpecs, job.JobSpec),
            buildingName = job.Building.name,
            colonistName = job.Colonist.Stats.name
        }).ToList();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.vacancyData != null)
        {
            vacancies.Clear();
            foreach (var row in data.vacancyData)
            {
                var building = BuildingController.Instance.FindBuildingByName(row.buildingName);
                var jobSpec = jobSpecs[row.jobSpecIndex];

                if (building != null)
                {
                    vacancies.Add(new Job(building, jobSpec));
                }
            }
        }

        if (data.assignedJobData != null)
        {
            assignedJobs.Clear();
            foreach (var row in data.assignedJobData)
            {
                var building = BuildingController.Instance.FindBuildingByName(row.buildingName);
                var colonist = AnimalController.Instance.FindAnimalByNameAndType<SmolbeanColonist>(row.colonistName);
                var jobSpec = jobSpecs[row.jobSpecIndex];

                if (building != null && colonist != null)
                {
                    var job = new Job(building, jobSpec) { Colonist = colonist };
                    assignedJobs.Add(job);
                    colonist.Job = job;
                }
            }
        }

        return null;
    }

    public void BuildingDestroyed(SmolbeanBuilding building)
    {
        var vacanciesToRemove = vacancies.Where(v => v.Building == building).ToArray();
        foreach (var job in vacanciesToRemove)
        {
            vacancies.Remove(job);
        }

        var jobsToTerminate = assignedJobs.Where(v => v.Building == building).ToArray();
        foreach (var job in jobsToTerminate)
        {
            job.Terminate();
            assignedJobs.Remove(job);
        }
    }
}
