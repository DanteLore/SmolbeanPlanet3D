using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGeneratorManager : MonoBehaviour
{
    public MapData mapData;
    public List<int> GameMap { get; private set; }
    public int GameMapWidth { get; private set; }
    public int GameMapHeight { get; private set; }
    public int MaxLevelNumber { get; private set; }

    void Awake()
    {
        BootstrapMapData();
    }

    public void BootstrapMapData()
    {
        if (mapData != null)
        {
            GameMapWidth = mapData.GameMapWidth;
            GameMapHeight = mapData.GameMapWidth;
            MaxLevelNumber = mapData.MaxLevelNumber;
            GameMap = mapData.GameMap.ToList();
        }
    }

    public IEnumerator Recreate(List<int> gameMap, int width, int height, bool newGame)
    {
        DateTime startTime = DateTime.Now;
        UnityEngine.Random.InitState(1);

        GameMapWidth = width;
        GameMapHeight = height;
        MaxLevelNumber = gameMap.Max();
        GameMap = gameMap;

        foreach (var gen in GetComponentsInChildren<IObjectGenerator>().OrderByDescending(g => g.Priority))
            gen.Clear();

        yield return null;
        Debug.Log($"Map cleared at {(DateTime.Now - startTime).TotalSeconds}s");

        var generators =
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<IObjectGenerator>()
            .Where(og => !og.RunModeOnly || Application.isPlaying)
            .Where(og => !og.NewGameOnly || newGame);

        foreach (var gen in generators.OrderBy(g => g.Priority))
        {
            yield return RunGeneratorTimed(gen);
        }

        Debug.Log($"Map generated in {(DateTime.Now - startTime).TotalSeconds}s");
    }

    private IEnumerator RunGeneratorTimed(IObjectGenerator gen)
    {
        DateTime startTime = DateTime.Now;
        yield return null;
        //Debug.Log($"Generator starting: {gen.GetType().Name}");
        yield return gen.Generate(GameMap, GameMapWidth, GameMapHeight);
        Debug.Log($"P{gen.Priority} Generator complete: {gen.GetType().Name} {(DateTime.Now - startTime).TotalSeconds}s");
        yield return null;
    }
}
