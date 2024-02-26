using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSpec", menuName = "Smolbean/Animal Spec", order = 2)]
public class AnimalSpec : ScriptableObject
{
    [Header("Basic")]
    public string animalName;
    public GameObject prefab;

    [Header("Species Settings")]
    public int startingPopulation;

    [Header("Vital Statistics")]
    public float initialHealth;
    public float maxHealth;
    public float healthRecoveryPerSecond;
    public float oldAgeSeconds;
    public float lifespanSeconds;
    public float oldAgeHealthImpactPerSecond;

    [Header("Senses and Abilities")]
    public float sightRange;
    public float speed;

    [Header("Food and Eating")]
    public float maxFoodLevel;
    public float initialFoodLevelMin;
    public float initialFoodLevelMax;
    public float foodDigestedPerSecond;
    public float starvationThreshold;
    public float starvationRatePerSecond;
    public float foodEatenPerSecond;
    public float fullThreshold;
    public float hungryThreshold;
    public float grassWearPerSecondWhenEating;
}
