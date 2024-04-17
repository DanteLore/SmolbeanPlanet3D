using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataCollectionManager : MonoBehaviour, IObjectGenerator
{
    public static DataCollectionManager Instance;

    public int Priority { get { return 500; } }
    public bool RunModeOnly { get { return true; } }

    public List<DataCollectionSeries> Series {get; private set; } = new ();

    private void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Clear()
    {
        StopCoroutine(nameof(GetData));

        Series.Clear();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        var sine = new DataCollectionSeries();
        Series.Add(sine);
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
        yield return new WaitForSeconds(1f);

        while(true)
        {
            float x = Mathf.Sin(Time.time / 10f);
            Series[0].AddValue(x);
            Debug.Log(x);

            yield return new WaitForSeconds(1f);
        }
    }
}
