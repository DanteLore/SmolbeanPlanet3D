using UnityEngine;

[CreateAssetMenu(fileName = "AnimalSpec", menuName = "Smolbean/Animal Spec", order = 2)]
public class AnimalSpec : ScriptableObject
{
    [Header("Basic")]
    public string animalName;
    public int prefabIndex;
    public DropSpec dropSpec;
    public GameObject birthParticleSystem;
    public GameObject deathParticleSystem;
    public GameObject sleepParticleSystem;
    public Texture2D thumbnail;

    [Header("Species Settings")]
    public int startingPopulation;
    public int populationCap;

    [Header("Vital Statistics")]
    public float initialHealth;
    public float maxHealth;
    public float oldAgeSeconds;
    public float lifespanSeconds;
    public float maturityAgeSeconds;

    [Header("Reproduction")]
    public float gestationPeriodSeconds;
    [Range(0f, 1f)] public float birthProbability;
    public float minimumHealthToGiveBirth;
    public float pregnancyHealthImpact;

    [Header("Senses and Abilities")]
    public float sightRange;
    public float minSpeed;
    public float maxSpeed;

    [Header("Food and Eating")]
    public float maxFoodLevel;
    public float initialFoodLevelMin;
    public float initialFoodLevelMax;
    public float starvationThreshold;
    public float foodEatenPerSecond;
    public float fullThreshold;
    public float hungryThreshold;
    public float grassWearPerSecondWhenEating;

    [Header("Buffs")]
    public BuffSpec[] Buffs;
}
