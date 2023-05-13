using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;  

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    public static readonly string EXTENSION = ".sbp";

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void SaveGame(string name)
    {
        string filename = GetFilename(name);
        Debug.Log($"Saving game to {filename}");

        if(File.Exists(filename))
            File.Delete(filename);

        // Things to be saved:
        // 1. The game map (which allows regeneration of the island mesh)
        // 2. The locations, rotations and scale of all the trees
        // 3. The locations, rotations and scale of all the buildings
        // 4. The location and state of all the creatures etc - future.
        // 5. Likewise items (wood, stone etc), stats, history etc etc

        using (StreamWriter file = File.CreateText(filename))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, GameObject.FindAnyObjectByType<GridManager>().GameMap);
        }
    }

    private static string GetFilename(string name)
    {
        string cleanName = name;
        foreach (var illegal in Path.GetInvalidFileNameChars())
            cleanName = cleanName.Replace(illegal, '_');
        cleanName += EXTENSION;
        string filename = Path.Join(Application.persistentDataPath, cleanName);
        return filename;
    }

    public IEnumerable<string> ListSaveFiles()
    {
        DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
        var files = info.GetFiles()
            .Where(f => f.Extension == EXTENSION)
            .OrderByDescending(f => f.CreationTime)
            .Select(f => Path.GetFileNameWithoutExtension(f.Name));
        return files;
    }

    internal static void LoadGame(string name)
    {
        string filename = GetFilename(name);
        Debug.Log($"Loading game from {filename}");

        using (StreamReader file = File.OpenText(filename))
        {
            JsonSerializer serializer = new JsonSerializer();
            List<int> map = (List<int>)serializer.Deserialize(file, typeof(List<int>));
            GameObject.FindAnyObjectByType<GridManager>().Recreate(map, 100, 100);
            MenuController.Instance.CloseAll();
        }

    }
}
