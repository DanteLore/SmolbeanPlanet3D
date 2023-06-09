using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingSpec", menuName = "Smolbean/Building Spec", order = 1)]
public class BuildingSpec : ScriptableObject
{
    public string buildingName;

    public GameObject prefab;

    public Texture2D thumbnail;
}
