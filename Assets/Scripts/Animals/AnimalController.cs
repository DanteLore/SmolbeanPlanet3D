using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.AI;

public class AnimalController : MonoBehaviour, IObjectGenerator
{
    public static AnimalController Instance { get; private set; }

    public AnimalSpec[] animalSpecs;
    public int Priority { get { return 12; } }
    public bool NewGameOnly { get { return true; } }
    public bool RunModeOnly { get { return true; } }
    public float edgeBuffer = 0.1f;
    public string natureLayer = "Nature";

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

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
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

                // Pick a random spot on the tile, not to close to the edge
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
            }
        }
    }

    public void CreateAnimal(AnimalSpec species, Vector3 pos)
    {
        var animalData = GenerateAnimalData(pos, species);
        InstantiateAnimal(animalData);
    }

    private void InstantiateAnimal(AnimalSaveData saveData)
    {
        var prefab = animalSpecs[saveData.speciesIndex].prefab;
        var pos = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        var rot = Quaternion.Euler(0f, saveData.rotationY, 0f);
        var animal = Instantiate(prefab, pos, rot, transform).GetComponent<SmolbeanAnimal>();
        animal.speciesIndex = saveData.speciesIndex;
        animal.species = animalSpecs[saveData.speciesIndex];
        animal.InitialiseStats(saveData.stats);
    }

    private AnimalSaveData GenerateAnimalData(Vector3 pos, AnimalSpec species)
    {
        int speciesIndex = Array.IndexOf(animalSpecs, species);

        float rotationY = Random.Range(0f, 360f);

        return new AnimalSaveData
        {
            positionX = pos.x,
            positionY = pos.y,
            positionZ = pos.z,
            rotationY = rotationY,
            speciesIndex = speciesIndex
        };
    }

    public List<AnimalSaveData> GetSaveData()
    {
        return GetComponentsInChildren<SmolbeanAnimal>()
            .Select(animal => animal.GetSaveData())
            .ToList();
    }

    public void LoadAnimals(List<AnimalSaveData> animalData)
    {
        Clear();

        animalData.ForEach(InstantiateAnimal);
    }

    public int AnimalCount(AnimalSpec species)
    {
        // This gon' be slow
        return GetComponentsInChildren<SmolbeanAnimal>().Count(animal => animal.species == species);
    }
}
