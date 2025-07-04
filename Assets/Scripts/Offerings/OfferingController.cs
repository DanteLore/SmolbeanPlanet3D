using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class OfferingController : MonoBehaviour, IObjectGenerator
{
    public int maxConcurrentOfferings = 10;

    public static OfferingController Instance { get; private set; }
    public int Priority { get { return 100; } }
    public bool RunModeOnly { get { return true; } }

    public DropSpec[] itemSpecs;
    public List<Offering> Offerings { get; } = new();

    public Action<Offering> OnOfferingCreated;
    public Action<Offering> OnOfferingExpired;
    public Action<Offering> OnOfferingCompleted;

    private float lastUpdate;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        if (GameStateManager.Instance.IsPaused)
            return;

        if(Time.time - lastUpdate < 1f) // Once a second is enough here!
            return;

        lastUpdate = Time.time;

        CheckForExpired();

        if (Offerings.Count < maxConcurrentOfferings &&
                Random.Range(0.0f, 1.0f) < 1.0f / 20.0f &&
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
        float lengthOfADay = DayNightCycleController.Instance.DayLengthSeconds;
        float availableDuration = lengthOfADay * Random.Range(0.5f, 1.5f); // Half to one and a half days
        float completionDuration = lengthOfADay * Random.Range(1.0f, 3.0f); // One to three days
        float ritualDuration = Random.Range(30.0f, 60.0f); // 30-60s
        float reward = Random.Range(100f, 200f);
        int count = Random.Range(1, itemSpecs.Length);
        var items = new List<OfferingPart>(count);

        var chosenDrops = itemSpecs.OrderBy(_ => Random.Range(1.0f, 2.0f)).Take(count).ToList();
        foreach(var item in chosenDrops)
        {
            var quantity = Random.Range(5, 20);
            items.Add(new OfferingPart(item.dropName, quantity));
        }

        var o = new Offering(availableDuration, completionDuration, ritualDuration, reward, items.ToArray());
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

    public DropSpec GetItemSpecFor(string itemName)
    {
        return itemSpecs.FirstOrDefault(i => i.dropName == itemName);
    }

    public void Complete(Offering offering)
    {
        Offerings.Remove(offering);
        OnOfferingCompleted?.Invoke(offering);
    }
}
