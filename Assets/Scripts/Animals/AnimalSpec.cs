using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSpec", menuName = "Smolbean/Animal Spec", order = 2)]
public class AnimalSpec : ScriptableObject
{
    [Header("Basic")]
    public string animalName;
    public GameObject prefab;
    public DropSpec dropSpec;
    public GameObject deathParticleSystem;
    public GameObject sleepParticleSystem;

    [Header("Species Settings")]
    public int startingPopulation;
    public int populationCap;

    [Header("Vital Statistics")]
    public float initialHealth;
    public float maxHealth;
    public float healthRecoveryPerSecond;
    public float sleepingHealthDecreaseMultiplier;
    public float oldAgeSeconds;
    public float lifespanSeconds;
    public float oldAgeHealthImpactPerSecond;
    public float maturityAgeSeconds;
    [Range(0f, 1f)] public float juvenileScale;

    [Header("Reproduction")]
    public float gestationPeriodSeconds;
    [Range(0f, 1f)] public float birthProbability;
    public float minimumHealthToGiveBirth;
    public float pregnancyHealthImpact;

    [Header("Senses and Abilities")]
    public float sightRange;
    public float speed;
    public float oldAgeSpeedDecrease;

    [Header("Food and Eating")]
    public float maxFoodLevel;
    public float initialFoodLevelMin;
    public float initialFoodLevelMax;
    public float foodDigestedPerSecond;
    public float sleepingDigestionMultiplier;
    public float starvationThreshold;
    public float starvationRatePerSecond;
    public float foodEatenPerSecond;
    public float fullThreshold;
    public float hungryThreshold;
    public float grassWearPerSecondWhenEating;
}
