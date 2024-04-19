using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataCollectionManager : MonoBehaviour, IObjectGenerator
{
    public static DataCollectionManager Instance;
    
    public int Priority { get { return 500; } }
    public bool RunModeOnly { get { return true; } }

    public List<DataCollectionSeries> Series {get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Clear()
    {
        StopCoroutine(nameof(FetchDataLoop));

        Series = null;
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        Series = FindObjectsByType<DataCollectionSeries>(sortMode: FindObjectsSortMode.None).OrderBy(s => s.displayName).ToList();

        StartCoroutine(nameof(FetchDataLoop));
        
        yield return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
    }

    private IEnumerator FetchDataLoop()
    {
        yield return new WaitForSeconds(1f);
        Debug.Assert(Series.Count < 30, "Getting enough data collection series to need a new algo here!");

        while(true)
        {
            float startTime = Time.time;
            foreach(var series in Series)
            {
                series.AddValue();
                yield return null;
            }
            float timeElapsed =  Time.time - startTime;

            yield return new WaitForSeconds(1f - timeElapsed);
        }
    }
}
