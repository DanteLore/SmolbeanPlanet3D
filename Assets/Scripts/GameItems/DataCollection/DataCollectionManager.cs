using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollectionManager : MonoBehaviour, IObjectGenerator
{
    public int Priority { get { return 500; } }
    public bool RunModeOnly { get { return true; } }

    private readonly List<DataCollectionSeries> series = new ();

    public void Clear()
    {
        StopCoroutine(nameof(GetData));

        series.Clear();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        var sine = new DataCollectionSeries();
        series.Add(sine);
        StartCoroutine(nameof(GetData));
        
        yield return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
    }

    private IEnumerator GetData()
    {
        yield return new WaitForSeconds(1);

        while(true)
        {
            series[0].AddValue(Mathf.Sin(Time.time / 60f));

            yield return new WaitForSeconds(1);
        }
    }
}
