using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Random = UnityEngine.Random;

public class AnimalController : MonoBehaviour, IObjectGenerator
{
    public static AnimalController Instance { get; private set; }

    public string animalLayer = "Creatures";
    public string uiLayer = "UI";
    public AnimalSpec[] animalSpecs;

    public GameObject[] animalPrefabs;
    public int Priority { get { return 150; } }
    public bool RunModeOnly { get { return true; } }
    public float edgeBuffer = 0.1f;

    public string natureLayer = "Nature";
    public int animalsToAddPerFrame = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        var gridManager = FindAnyObjectByType<GridManager>();

        foreach (var species in animalSpecs)
        {
            int animalsToAdd = species.startingPopulation;

            while(animalsToAdd > 0)
            {
                // Pick a random square
                int x = Random.Range(0, gameMapWidth);
                int z = Random.Range(0, gameMapHeight);

                // Chech it's not water
                if (gameMap[z * gameMapWidth + x] == 0)
                    continue;

                // Pick a random spot on the tile, not too close to the edge
                Rect squareBounds = gridManager.GetSquareBounds(x, z);
                float buffer = gridManager.tileSize * edgeBuffer;
                float worldX = Random.Range(squareBounds.xMin + buffer, squareBounds.xMax - buffer);
                float worldZ = Random.Range(squareBounds.yMin + buffer, squareBounds.yMax - buffer);

                // Get the Y coord at this spot
                bool hit = Physics.Raycast(new Ray(new Vector3(worldX, 1000, worldZ), Vector3.down), out RaycastHit rayHit, 2000, LayerMask.GetMask("Ground"));

                // Check we hit the ground
                if (!hit)
                    continue;

                float worldY = rayHit.point.y;

                // Create candidate position
                var pos = new Vector3(worldX, worldY, worldZ);

                // Check no nature or animals are too close
                if (Physics.CheckSphere(pos, 1f, LayerMask.GetMask("Creatures", "Nature")))
                    continue;

                // Check the spot is on the navmesh
                NavMesh.SamplePosition(pos, out var navHit, 100f, NavMesh.AllAreas);
                if (navHit.distance > 0.1f)
                    continue;

                // Finally, create the animal
                CreateAnimal(species, pos);
                animalsToAdd--;

                if(animalsToAdd % animalsToAddPerFrame == 0)
                    yield return null; 
            }
        }
    }

    public T FindAnimalByNameAndType<T>(string name) where T : SmolbeanAnimal
    {
        return GetComponentsInChildren<T>().FirstOrDefault(x => x.Stats.name == name);
    }

    public IEnumerable<T> GetAnimalsByType<T>() where T : SmolbeanAnimal
    {
        return GetComponentsInChildren<T>();
    }

    public SmolbeanAnimal CreateAnimal(AnimalSpec species, Vector3 pos)
    {
        var animalData = GenerateAnimalData(pos, species);
        if(species.birthParticleSystem)
            Instantiate(species.birthParticleSystem, pos, Quaternion.identity);
        return InstantiateAnimal(animalData);
    }

    private SmolbeanAnimal InstantiateAnimal(AnimalSaveData saveData)
    {
        var spec = animalSpecs[saveData.speciesIndex];

        Debug.Assert(spec != null, $"Error creating animal with data {saveData}");

        var prefabIndex = saveData.prefabIndex;
        var prefab = animalPrefabs[prefabIndex];
        var pos = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        var rot = Quaternion.Euler(0f, saveData.rotationY, 0f);
        var animal = Instantiate(prefab, pos, rot, transform).GetComponent<SmolbeanAnimal>();
        animal.Target = pos;
        animal.Species = spec;
        animal.LoadFrom(saveData);

        return animal;
    }

    public void SwitchAnimal(SmolbeanAnimal original, GameObject prefab)
    {
        original.gameObject.SetActive(false);

        Vector3 pos = original.transform.position;
        Quaternion rot = original.transform.rotation;
        Transform parent = original.transform.parent;

        original.DropInventory();

        var newAnimal = Instantiate(prefab, pos, rot, parent).GetComponent<SmolbeanAnimal>();
        newAnimal.AdoptIdentity(original);
        newAnimal.SpeciesIndex = Array.IndexOf(animalSpecs, newAnimal.Species);
        newAnimal.PrefabIndex = Array.IndexOf(animalPrefabs, prefab);

        Destroy(original.gameObject);
    }

    private AnimalSaveData GenerateAnimalData(Vector3 pos, AnimalSpec species)
    {
        int speciesIndex = Array.IndexOf(animalSpecs, species);

        float rotationY = Random.Range(0f, 360f);

        // Buffs
        var newBuffs = new List<BuffInstance>();
        foreach (var buffSpec in species.Buffs)
        {
            newBuffs.Add(buffSpec.GetBuff());
        }

        return new AnimalSaveData
        {
            positionX = pos.x,
            positionY = pos.y,
            positionZ = pos.z,
            rotationY = rotationY,
            speciesIndex = speciesIndex,
            prefabIndex = species.prefabIndex,
            thoughts = Array.Empty<Thought>(),
            buffs = newBuffs.ToArray()
        };
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.animalData = GetComponentsInChildren<SmolbeanAnimal>()
            .Select(animal => animal.GetSaveData())
            .ToList();
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        if (data.animalData != null)
            foreach(var d in data.animalData)
                InstantiateAnimal(d);
        
        return null;
    }

    public int AnimalCount(AnimalSpec species)
    {
        return GetComponentsInChildren<SmolbeanAnimal>().Count(animal => animal.Species == species);
    }

    public int AnimalCount()
    {
        return GetComponentsInChildren<SmolbeanAnimal>().Count();
    }
}
