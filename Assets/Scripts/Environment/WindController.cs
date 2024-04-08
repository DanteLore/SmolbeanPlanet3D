using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour, IObjectGenerator
{
    public static WindController Instance { get; private set; }

    public float WindDirection { get; private set; }
    public int Priority { get { return 5; } }
    public bool RunModeOnly { get { return true; } }

    public Vector3 WindVector
    {
        get
        {
            return WindRotation * Vector3.forward;
        }
    }

    public Quaternion WindRotation
    {
        get
        {
            return Quaternion.AngleAxis(WindDirection, Vector3.up);
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Update()
    {
        if (GameStateManager.Instance.IsStarted)
        {
            float dW = Mathf.Sin(Time.time * 0.001f) * 0.5f;
            dW += Mathf.Sin(Time.time * 0.002f) * 0.3f;
            dW += Mathf.Sin(Time.time * 0.01f) * 0.1f;

            WindDirection = 180f * dW;
        }
    }

    public void Clear()
    {
        //
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.windData = new WindSaveData { windDirection = WindDirection };
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.windData != null)
        {
            WindDirection = data.windData.windDirection;
        }

        return null;
    }
}
