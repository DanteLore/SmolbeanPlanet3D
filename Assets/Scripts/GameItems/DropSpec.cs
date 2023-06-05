using UnityEngine;

[CreateAssetMenu(fileName = "DropSpec", menuName = "Smolbean/Drop Spec", order = 1)]
public class DropSpec : ScriptableObject
{
    public string dropName;

    public int dropRate = 4;
    public int stackSize = 8;

    public Texture2D thumbnail;
    public GameObject singlePrefab;
    public GameObject somePrefab;
    public GameObject lotsPrefab;
}
