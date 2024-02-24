using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSpec", menuName = "Smolbean/Animal Spec", order = 2)]
public class AnimalSpec : ScriptableObject
{
    public string animalName;
    public GameObject prefab;
    public int startingPopulation;
}
