using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System.Collections;
using System;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    public static readonly string EXTENSION = ".sbp";
    public Texture2D groundTexture;
    private MapGeneratorManager mapGenerator;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        mapGenerator = FindAnyObjectByType<MapGeneratorManager>();
    }

    public void SaveGame(string name)
    {
        string filename = GetFilename(name);
        Debug.Log($"Saving game to {filename}");

        if (File.Exists(filename))
            File.Delete(filename);

        SaveFileData saveData = mapGenerator.Save(filename);

        using (StreamWriter file = File.CreateText(filename))
        {
            JsonSerializer serializer = new()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            serializer.Serialize(file, saveData);
        }

        PrefsManager.Instance.LastSaveName = name;
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
        DirectoryInfo info = new(Application.persistentDataPath);
        var files = info.GetFiles()
            .Where(f => f.Extension == EXTENSION)
            .OrderByDescending(f => f.CreationTime)
            .Select(f => Path.GetFileNameWithoutExtension(f.Name));
        return files;
    }

    public IEnumerator LoadGame(string name)
    {
        string filename = GetFilename(name);
        Debug.Log($"Loading game from {filename}");

        SaveFileData saveData = null;
        using (StreamReader file = File.OpenText(filename))
        {
            JsonSerializer serializer = new()
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            saveData = (SaveFileData)serializer.Deserialize(file, typeof(SaveFileData));
        }

        PrefsManager.Instance.LastSaveName = name;

        yield return mapGenerator.Load(saveData, filename);
    }

    public bool SaveFileExists(string name)
    {
        return File.Exists(GetFilename(name));
    }
}
