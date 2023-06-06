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
    public List<NatureObjectSaveData> rockData;
    public List<BuildingObjectSaveData> buildingData;
    public List<DropItemSaveData> dropItemData;
    public CameraSaveData cameraData;
}

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    public static readonly string EXTENSION = ".sbp";
    public Texture2D groundTexture;

    private GridManager gridManager;
    private TreeGenerator treeGenerator;
    private RockGenerator rockGenerator;
    private BuildManager buildManager;
    private DropController dropController;
    private CameraController cameraController;

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
        rockGenerator = GameObject.FindAnyObjectByType<RockGenerator>();
        buildManager = GameObject.FindAnyObjectByType<BuildManager>();
        dropController = GameObject.FindAnyObjectByType<DropController>();
        cameraController = GameObject.FindAnyObjectByType<CameraController>();
    }

    public void SaveGame(string name)
    {
        string filename = GetFilename(name);
        Debug.Log($"Saving game to {filename}");

        if(File.Exists(filename))
            File.Delete(filename);

        var saveData = new SaveFileData
        {
            gameMapWidth = gridManager.GameMapWidth,
            gameMapHeight = gridManager.GameMapHeight,
            gameMap = gridManager.GameMap,
            treeData = treeGenerator.GetSaveData(),
            rockData = rockGenerator.GetSaveData(),
            buildingData = buildManager.GetSaveData(),
            dropItemData = dropController.GetSaveData(),
            cameraData = cameraController.GetSaveData()
        };

        using (StreamWriter file = File.CreateText(filename))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(file, saveData);
        }

        if(groundTexture != null)
        {
            File.WriteAllBytes(GetPngFilename(filename), groundTexture.EncodeToPNG());
        }
    }

    private static string GetPngFilename(string filename)
    {
        return filename.Replace(EXTENSION, ".png");
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

        gridManager.Recreate(saveData.gameMap, saveData.gameMapWidth, saveData.gameMapHeight);
        
        if(saveData.treeData != null)
            treeGenerator.LoadTrees(saveData.treeData);
        if(saveData.rockData != null)
            rockGenerator.LoadRocks(saveData.rockData);
        if(saveData.buildingData != null)
            buildManager.LoadBuildings(saveData.buildingData);
        if(saveData.dropItemData != null)
            dropController.LoadDrops(saveData.dropItemData);
        if(saveData.cameraData != null)
            cameraController.LoadState(saveData.cameraData);

        string pngFilename = GetPngFilename(filename);
        if(File.Exists(pngFilename) && groundTexture != null)
        {
            groundTexture.LoadImage(File.ReadAllBytes(pngFilename));
        }

        MenuController.Instance.CloseAll();
    }
}
