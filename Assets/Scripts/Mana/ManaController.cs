using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaController : MonoBehaviour, IObjectGenerator
{
    public static ManaController Instance;

    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

    public Action<float> OnManaChanged;
    public float startingMana = 1000f;
    private float mana;
    private SoundPlayer soundPlayer;

    public float Mana
    {
        get => mana;
        private set
        {
            if (mana != value)
            {
                mana = value;
                soundPlayer.PlayOneShot("Magic1");
                OnManaChanged?.Invoke(mana);
            }
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

    private void Start()
    {
        soundPlayer = GameObject.Find("SFXManager").GetComponent<SoundPlayer>();
    }

    private void Update()
    {
        
    }

    public void AddMana(float amount)
    {
        Mana += amount;
    }
}
