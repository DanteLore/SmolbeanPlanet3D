using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Smolbean/TerrainData", order = 1)]
public class TerrainData : ScriptableObject
{
    [SerializeField] public float fuzzyEdgeFactor = 0.01f;
    [SerializeField] public float levelMeshHeight = 4.0f; 
    [SerializeField] public int MaxLevelNumber = 10;
    [SerializeField] public NeighbourData[] neighbourData;
    [SerializeField] public MeshData[] meshData;
}
