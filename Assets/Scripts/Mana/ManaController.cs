using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManaController : MonoBehaviour, IObjectGenerator
{
    public static ManaController Instance;

    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

    public Action<float> OnManaChanged;
    public float startingMana = 1000f;
    private float mana;

    public float Mana
    {
        get => mana;
        private set
        {
            mana = value;
            OnManaChanged?.Invoke(mana);
        }
    }

    public void Clear()
    {
        // Nothing to do here!
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        mana = startingMana;
        yield return null;
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        mana = data.mana;
        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.mana = mana;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Update()
    {
        if (Random.Range(0.0f, 1.0f) < 1.0f / 1000.0f)
        {
            // For now, just randomly add mana!
            Mana += Random.Range(1000f, 10000f);
        }
    }

}
