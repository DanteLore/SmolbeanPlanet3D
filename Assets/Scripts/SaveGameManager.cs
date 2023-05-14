using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;
using Newtonsoft.Json;  

public class SaveFileData
{
    public int gameMapWidth;
    public int gameMapHeight;
    public List<int> gameMap;
    public List<NatureObjectSaveData> treeData;
}

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    public static readonly string EXTENSION = ".sbp";

    private GridManager gridManager;
    private TreeGenerator treeGenerator;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        gridManager = GameObject.FindAnyObjectByType<GridManager>();
        treeGenerator = GameObject.FindAnyObjectByType<TreeGenerator>();
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

        var saveData = new SaveFileData
        {
            gameMapWidth = gridManager.GameMapWidth,
            gameMapHeight = gridManager.GameMapHeight,
            gameMap = gridManager.GameMap,
            treeData = treeGenerator.GetSaveData()
        };

        using (StreamWriter file = File.CreateText(filename))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, saveData);
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

    public void LoadGame(string name)
    {
        string filename = GetFilename(name);
        Debug.Log($"Loading game from {filename}");

        SaveFileData saveData = null;
        using (StreamReader file = File.OpenText(filename))
        {
            JsonSerializer serializer = new JsonSerializer();
            saveData = (SaveFileData)serializer.Deserialize(file, typeof(SaveFileData));
        }

        GameObject.FindAnyObjectByType<GridManager>().Recreate(saveData.gameMap, saveData.gameMapWidth, saveData.gameMapHeight);
        treeGenerator.LoadTrees(saveData.treeData);
        MenuController.Instance.CloseAll();
    }
}
