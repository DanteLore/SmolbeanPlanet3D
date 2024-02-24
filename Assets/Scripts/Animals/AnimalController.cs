using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalController : MonoBehaviour, IObjectGenerator
{
    public AnimalSpec[] animalSpecs;
    public int Priority { get { return 12; } }
    public float edgeBuffer = 0.1f;
    public string natureLayer = "Nature";

    private GridManager gridManager;

    void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public void Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        foreach(var species in animalSpecs)
        {
            int animalsToAdd = species.startingPopulation;

            while(animalsToAdd > 0)
            {
                int x = Random.Range(0, gameMapWidth);
                int z = Random.Range(0, gameMapHeight);

                if (gameMap[z * gameMapWidth + x] == 0)
                    continue;

                Rect squareBounds = gridManager.GetSquareBounds(x, z);
                bool boxFull = Physics.CheckBox(squareBounds.center, new Vector3(squareBounds.width / 2f, 100f, squareBounds.height / 2f), Quaternion.identity, LayerMask.GetMask(natureLayer));

                var animalData = GenerateAnimalData(gridManager, squareBounds, species);
                InstantiateAnimal(animalData);

                animalsToAdd--;
            }
        }
    }

    private void InstantiateAnimal(AnimalSaveData saveData)
    {
        var prefab = animalSpecs[saveData.speciesIndex].prefab;
        var pos = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        var rot = Quaternion.Euler(0f, saveData.rotationY, 0f);
        var animal = Instantiate(prefab, pos, rot, transform);
    }

    private AnimalSaveData GenerateAnimalData(GridManager gridManager, Rect squareBounds, AnimalSpec species)
    {
        int speciesIndex = Array.IndexOf(animalSpecs, species);

        float buffer = gridManager.tileSize * edgeBuffer;
        float worldX = Random.Range(squareBounds.xMin + buffer, squareBounds.xMax - buffer);
        float worldZ = Random.Range(squareBounds.yMin + buffer, squareBounds.yMax - buffer);
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

        float rotationY = Random.Range(0f, 360f);

        return new AnimalSaveData
        {
            positionX = worldX,
            positionY = worldY,
            positionZ = worldZ,
            rotationY = rotationY,
            speciesIndex = speciesIndex
        };
    }
}
