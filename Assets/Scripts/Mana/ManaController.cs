using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManaController : MonoBehaviour
{
    public static ManaController Instance;

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
