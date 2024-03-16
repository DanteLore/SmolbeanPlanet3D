using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobController : MonoBehaviour, IObjectGenerator
{
    public static JobController Instance { get; private set; }

    public GameObject freeColonistPrefab;

    private List<Job> vacancies = new();
    private List<Job> assignedJobs = new();

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

    public void RegisterJob(JobSpec jobSpec, SmolbeanBuilding woodcuttersHut)
    {
        Job job = new(woodcuttersHut, jobSpec);

        vacancies.Add(job);
    }

    public void Clear()
    {
        vacancies.Clear();
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        //
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
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
