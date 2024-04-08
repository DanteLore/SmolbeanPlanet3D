using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour, IObjectGenerator
{
    public static WindController Instance { get; private set; }

    [Range(0f, 1f)] public float amp1 = 0.5f;
    [Range(0f, 1f)] public float amp2 = 0.3f;
    [Range(0f, 1f)] public float amp3 = 0.1f;
    [Range(0f, 0.1f)] public float freq1 = 0.001f;
    [Range(0f, 0.1f)] public float freq2 = 0.002f;
    [Range(0f, 0.1f)] public float freq3 = 0.01f;

    public Material[] materials;

    public float WindDirection { get; private set; }
    public int Priority { get { return 5; } }
    public bool RunModeOnly { get { return true; } }

    private float windTimeOffset;

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
            float t = windTimeOffset + Time.time;
            float dW = Mathf.Sin(t * freq1) * amp1;
            dW += Mathf.Sin(t * freq2) * amp2;
            dW += Mathf.Sin(t * freq3) * amp3;

            WindDirection = 180f * (1f + dW);
        }

        Vector3 normalized = WindVector.normalized;
        foreach (var mat in materials)
            mat.SetVector("_windDirection", normalized);
    }

    public void Clear()
    {
        //
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        windTimeOffset = 0f;
        return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.windData = new WindSaveData { windTimeOffset = windTimeOffset };
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.windData != null)
        {
            windTimeOffset = data.windData.windTimeOffset;
        }

        return null;
    }
}
