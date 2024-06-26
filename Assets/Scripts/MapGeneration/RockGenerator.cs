using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class RockGenerator : MonoBehaviour, IObjectGenerator
{
    [Serializable]
    public struct RockData
    {
        public GameObject prefab;
        public float probability;
    }

    public int Priority { get { return 20; } }
    public bool RunModeOnly { get { return false; } }

    public RockData[] rockData;
    public float rocksPerGridSquareFactor = 0.1f;
    public int maxRocksPerSquare = 5;
    public float edgeBuffer = 0.1f;
    public float scaleMax = 1.2f;
    public float scaleMin = 0.8f;
    public float tiltMaxDegrees = 6.0f;
    public string natureLayer = "Nature";
    public bool rocksInWaterEnabled = false;
    public int rocksToGeneratePerFrame = 100;

    public MapData mapData;

    public IEnumerator Generate(List<int> gameMap, int gameMapWidth, int gameMapHeight)
    {
        Clear();
        var gridManager = FindAnyObjectByType<GridManager>();

        int numberOfRocksToGenerate = Mathf.FloorToInt(gameMapWidth * gameMapHeight * rocksPerGridSquareFactor);
        
        while(numberOfRocksToGenerate > 0)
        {
            int x = UnityEngine.Random.Range(0, gameMapWidth);
            int z = UnityEngine.Random.Range(0, gameMapHeight);

            if(!rocksInWaterEnabled && gameMap[z * gameMapWidth + x] == 0)
                continue;

            Rect squareBounds = gridManager.GetSquareBounds(x, z);
            bool boxFull = Physics.CheckBox(squareBounds.center, new Vector3(squareBounds.width / 2f, 100f, squareBounds.height / 2f), Quaternion.identity, LayerMask.GetMask(natureLayer));

            if(!boxFull)
            {
                int rockCount = UnityEngine.Random.Range(0, maxRocksPerSquare);

                while(rockCount-- > 0)
                {
                    var rockData = GenerateRockData(gridManager, squareBounds);
                    InstantiateRock(rockData);

                    numberOfRocksToGenerate--;
                    if (numberOfRocksToGenerate % rocksToGeneratePerFrame == 0)
                        yield return null;
                }
            }
        }
        yield return null;
    }

    private NatureObjectSaveData GenerateRockData(GridManager gridManager, Rect squareBounds)
    {
        int rockIndex = UnityEngine.Random.Range(0, rockData.Length);

        float buffer = gridManager.tileSize * edgeBuffer;
        float worldX = UnityEngine.Random.Range(squareBounds.xMin + buffer, squareBounds.xMax - buffer);
        float worldZ = UnityEngine.Random.Range(squareBounds.yMin + buffer, squareBounds.yMax - buffer);
        float worldY = gridManager.GetGridHeightAt(worldX, worldZ);

        float rotationY = UnityEngine.Random.Range(0f, 360f);
        float rotationX = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);
        float rotationZ = UnityEngine.Random.Range(-tiltMaxDegrees, tiltMaxDegrees);

        float scaleX = UnityEngine.Random.Range(scaleMin, scaleMax);
        float scaleY = UnityEngine.Random.Range(scaleMin, scaleMax);
        float scaleZ = UnityEngine.Random.Range(scaleMin, scaleMax);

        return new NatureObjectSaveData
        {
            positionX = worldX,
            positionY = worldY,
            positionZ = worldZ,
            rotationX = rotationX,
            rotationY = rotationY,
            rotationZ = rotationZ,
            scaleX = scaleX,
            scaleY = scaleY,
            scaleZ = scaleZ,
            prefabIndex = rockIndex
        };
    }

    private void InstantiateRock(NatureObjectSaveData saveData)
    {
        var prefab = rockData[saveData.prefabIndex].prefab;
        var pos = new Vector3(saveData.positionX, saveData.positionY, saveData.positionZ);
        var rot = Quaternion.Euler(saveData.rotationX, saveData.rotationY, saveData.rotationZ);
        var rock = Instantiate(prefab, pos, rot, transform);
        rock.transform.localScale = new Vector3(saveData.scaleX, saveData.scaleY, saveData.scaleZ);
        rock.GetComponent<SmolbeanRock>().saveData = saveData;
    }

    public void Clear()
    {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    public IEnumerator Load(SaveFileData data, string filename)
    {
        data.rockData?.ForEach(InstantiateRock);

        return null;
    }

    public void SaveTo(SaveFileData saveData, string filename)
    {
        saveData.rockData = GetComponentsInChildren<SmolbeanRock>().Select(t => t.saveData).ToList();
    }
}
