using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class OfferingController : MonoBehaviour, IObjectGenerator
{
    public static OfferingController Instance { get; private set; }
    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

    public DropSpec[] itemSpecs;
    public List<Offering> Offerings { get; } = new();

    public Action<Offering> OnOfferingCreated;
    public Action<Offering> OnOfferingExpired;
    public Action<Offering> OnOfferingCompleted;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        CheckForExpired();

        if (Offerings.Count == 0 &&
                Random.Range(0.0f, 1.0f) < 1.0f / 1000.0f &&
                !GameStateManager.Instance.IsPaused)
        {
            CreateRandomOffering();
        }
    }

    private void CheckForExpired()
    {
        for (int i = Offerings.Count - 1; i >= 0; i--)
        {
            var o = Offerings[i];
            if (o.IsExpired)
            {
                OnOfferingExpired?.Invoke(o);
                Offerings.RemoveAt(i);
            }
        }
    }

    private void CreateRandomOffering()
    {
        float availableDuration = Random.Range(30f, 60f);
        float completionDuration = Random.Range(30f, 60f);
        float reward = Random.Range(100f, 200f);
        int count = Random.Range(1, 3);
        var items = new List<OfferingPart>(count);

        for (int i = 0; i < count; i++)
        {
            var item = itemSpecs[Random.Range(0, itemSpecs.Length)];
            var quantity = Random.Range(5, 20);
            items.Add(new OfferingPart(item.dropName, quantity));
        }

        var o = new Offering(availableDuration, completionDuration, reward, items.ToArray());
        Offerings.Add(o);
        OnOfferingCreated?.Invoke(o);
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.offerings != null)
            Offerings.AddRange(data.offerings);

        yield return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.offerings = Offerings;
    }

    public void Clear()
    {
        Offerings.Clear();
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        // Nothing to do here!
        yield return null;
    }
}
