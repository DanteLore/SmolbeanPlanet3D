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

    public IEnumerator Recreate(List<int> gameMap, int width, int height)
    {
        DateTime startTime = DateTime.Now;
        UnityEngine.Random.InitState(1);

        GameMapWidth = width;
        GameMapHeight = height;
        MaxLevelNumber = gameMap.Max();
        GameMap = gameMap;

        Clear();
        Debug.Log($"Map cleared at {(DateTime.Now - startTime).TotalSeconds}s");
        yield return null;

        foreach (var gen in GetActiveGenerators().OrderBy(g => g.Priority))
            yield return RunGeneratorTimed(gen);

        Debug.Log($"Map generated in {(DateTime.Now - startTime).TotalSeconds}s");
    }

    public IEnumerator Load(SaveFileData saveData)
    {
        DateTime startTime = DateTime.Now;
        UnityEngine.Random.InitState(1);

        GameMapWidth = saveData.gameMapWidth;
        GameMapHeight = saveData.gameMapHeight;
        GameMap = saveData.gameMap;
        MaxLevelNumber = GameMap.Max();

        Clear();
        Debug.Log($"Map cleared at {(DateTime.Now - startTime).TotalSeconds}s");
        yield return null;

        foreach (var gen in GetActiveGenerators().OrderBy(g => g.Priority))
            yield return LoadGeneratorTimed(gen, saveData);

        Debug.Log($"Map generated in {(DateTime.Now - startTime).TotalSeconds}s");
    }

    public SaveFileData Save()
    {
        SaveFileData saveData = new();

        foreach (var gen in GetAllGenerators())
            gen.SaveTo(saveData);

        return saveData;
    }

    private static IEnumerable<IObjectGenerator> GetActiveGenerators()
    {
        return GetAllGenerators()
            .Where(og => !og.RunModeOnly || Application.isPlaying);
    }

    private static IEnumerable<IObjectGenerator> GetAllGenerators()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .OfType<IObjectGenerator>();
    }

    public void Clear()
    {
        foreach (var gen in GetAllGenerators().OrderByDescending(x => x.Priority))
            gen.Clear();
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

    private IEnumerator LoadGeneratorTimed(IObjectGenerator gen, SaveFileData saveData)
    {
        DateTime startTime = DateTime.Now;
        yield return null;
        //Debug.Log($"Generator starting: {gen.GetType().Name}");
        yield return gen.Load(saveData);
        Debug.Log($"P{gen.Priority} Load complete: {gen.GetType().Name} {(DateTime.Now - startTime).TotalSeconds}s");
        yield return null;
    }
}
