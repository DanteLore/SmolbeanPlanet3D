using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobController : MonoBehaviour, IObjectGenerator
{
    public static JobController Instance { get; private set; }

    private List<Job> jobs = new();

    public IEnumerable<Job> Jobs { get { return jobs; } }
    public int Priority { get { return 150; } }
    public bool RunModeOnly { get { return true; } }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void RegisterJob(JobSpec jobSpec, SmolbeanBuilding woodcuttersHut)
    {
        Job job = new(woodcuttersHut, jobSpec);

        jobs.Add(job);
    }

    public void Clear()
    {
        jobs.Clear();
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
}
