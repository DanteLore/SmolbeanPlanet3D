using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System;

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

        File.Create(filename).Close(); // Literally just an empty file for now :)
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
    }
}
