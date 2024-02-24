using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSpec", menuName = "Smolbean/Animal Spec", order = 2)]
public class AnimalSpec : ScriptableObject
{
    public string animalName;
    public GameObject prefab;
    public int startingPopulation;
    public float maxFoodLevel;
    public float initialFoodLevel;
    public float foodDigestedPerSecond;
    public float maxHealth;
    public float initialHealth;
    public float starvationThreshold;
    public float starvationRatePerSecond;
    public float healthRecoveryPerSecond;
    public float foodEatenPerSecond;
}
